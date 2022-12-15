using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class EventEditor : EditorWindow
{
	SerializedObject serializedObject;
	ReorderableList stepList, elementList;
	int selectedStep, selectedElement;		// what's focused in the list and will be displayed next to or under it
	SerializedProperty elementProperty;		// "Draw" callbacks have fixed signatures so we set the element to draw here
	bool drawElement = true;                // prevents issues with deleting and reordering elements

	static bool conditionsFoldout = true;
	GUIStyle labelStyle;

	public static void OpenWindow(Event ev)
	{
		EventEditor window = GetWindow<EventEditor>("Event Editor");
		window.serializedObject = new SerializedObject(ev, ev);
		//window.maxSize = new Vector2(850f, 500f);
		window.minSize = new Vector2(675f, 350f);

		window.stepList = new ReorderableList(window.serializedObject, window.serializedObject.FindProperty("steps"), true, true, true, true);
		window.stepList.drawHeaderCallback = window.DrawStepsHeader;
		window.stepList.drawElementCallback = window.DrawStepElement;		
		window.stepList.onAddCallback += window.AddStep;
		
		window.stepList.onSelectCallback += window.OnStepSelect;
		window.stepList.onChangedCallback += window.OnStepsChanged;

		window.elementList = new ReorderableList(window.serializedObject, null, true, false, true, true);

		window.labelStyle = new GUIStyle();
		window.labelStyle.fontStyle = FontStyle.Bold;
		window.labelStyle.normal.textColor = Color.white * 0.85f;
	}

	private void OnGUI()
	{
		serializedObject.Update();
		DrawLayout();
		serializedObject.ApplyModifiedProperties();
	}

	void DrawLayout()
	{
		EditorGUILayout.BeginHorizontal();
			// Must set both min and max width so shrinking elements in the right doesn't affect this part
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(290), GUILayout.MinWidth(290));
				stepList.DoLayoutList();
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Mood Change", "The default change in mood caused by the event"), labelStyle, GUILayout.Width(150));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("moodChange"), GUIContent.none);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Nb Times", "Number of times this event can happen"), labelStyle, GUILayout.Width(150));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("nbTimes"), GUIContent.none);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent("Require input", "Should this event require an input from the user to start?"), labelStyle, GUILayout.Width(150));
					EditorGUILayout.PropertyField(serializedObject.FindProperty("requireInput"), GUIContent.none);
				EditorGUILayout.EndHorizontal();				

				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredEvents"));
				DrawConditions();
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
				DrawStep();
			EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}

	// Draw the Condition list
	void DrawConditions()
	{
		GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
		foldoutStyle.normal.textColor = labelStyle.normal.textColor;
		foldoutStyle.fontStyle = labelStyle.fontStyle;

		conditionsFoldout = EditorGUILayout.Foldout(conditionsFoldout, "Conditions", true, foldoutStyle);
		if (!conditionsFoldout)
			return;

		SerializedProperty conditions = serializedObject.FindProperty("conditions");
		if(conditions.arraySize != (int)Conditions.count)
		{
			for (int i = 0; i < conditions.arraySize; i++)
				conditions.GetArrayElementAtIndex(i).boolValue = false;
			conditions.arraySize = (int)Conditions.count;
		}

		for (int i = 0; i < conditions.arraySize; i++)
		{
			Conditions name = (Conditions)i;
			EditorGUILayout.PropertyField(conditions.GetArrayElementAtIndex(i), new GUIContent(name.ToString()));
		}				
	}

	// Display all properties based on the step's type
	void DrawStep()
	{
		if (stepList.count == 0 || !drawElement)
			return;

		SerializedProperty step = stepList.serializedProperty.GetArrayElementAtIndex(selectedStep);
		SerializedProperty property;
		switch ((StepType)step.FindPropertyRelative("stepType").enumValueIndex)
		{
		case StepType.Dialogue:
			property = step.FindPropertyRelative("dialogues");
			elementProperty = property;

			elementList.drawElementCallback = DrawDialogueElement;
			elementList.onAddCallback = AddDialogueElement;
			elementList.serializedProperty = property;

			EditorGUILayout.Space(); EditorGUILayout.Space();
			elementList.DoLayoutList();
			NextStepProperty(step);

			if (elementList.count > 0)
			{
				// Check to set selected choice as the last in case previous selected was last and got deleted
				SerializedProperty dialogue = property.arraySize > selectedElement ? property.GetArrayElementAtIndex(selectedElement) : property.GetArrayElementAtIndex(property.arraySize - 1);
				
				EditorGUILayout.LabelField("Dialogue " + (selectedElement + 1), labelStyle);
				EditorGUILayout.PropertyField(dialogue.FindPropertyRelative("italic"));

				GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };       // default style doesn't have wordwrap...
				dialogue.FindPropertyRelative("text").stringValue = EditorGUILayout.TextArea(dialogue.FindPropertyRelative("text").stringValue, textAreaStyle, GUILayout.MaxHeight(200));
			}
			break;

		case StepType.Choice:
			property = step.FindPropertyRelative("choices");
			elementProperty = property;

			elementList.drawElementCallback = DrawChoiceElement;
			elementList.onAddCallback = AddChoiceElement;
			elementList.serializedProperty = property;

			EditorGUILayout.Space(); EditorGUILayout.Space();
			elementList.DoLayoutList();
			step.FindPropertyRelative("overrideNextStep").boolValue = false;

			if (elementList.count > 0)
			{
				SerializedProperty choice, toggle;
				choice = property.arraySize > selectedElement ? property.GetArrayElementAtIndex(selectedElement) : property.GetArrayElementAtIndex(property.arraySize - 1);

				EditorGUILayout.LabelField("Choice " + (selectedElement + 1), labelStyle);
				EditorGUILayout.PropertyField(choice.FindPropertyRelative("choiceText"), GUILayout.ExpandWidth(true));

				SerializedProperty minMood = choice.FindPropertyRelative("minMood");
				SerializedProperty maxMood = choice.FindPropertyRelative("maxMood");
				EditorGUILayout.PropertyField(minMood, GUILayout.MaxWidth(250));
				minMood.intValue = Mathf.Clamp(minMood.intValue, 0, 100);
				EditorGUILayout.PropertyField(maxMood, GUILayout.MaxWidth(250));
				maxMood.intValue = Mathf.Clamp(maxMood.intValue, 0, 100);

				EditorGUILayout.BeginHorizontal();
					toggle = choice.FindPropertyRelative("overrideMoodChange");
					EditorGUILayout.PropertyField(toggle, GUILayout.MaxWidth(200));
					if (toggle.boolValue)
						EditorGUILayout.PropertyField(choice.FindPropertyRelative("moodChange"), GUILayout.MaxWidth(250));
					else
					{
						GUI.enabled = false;
						EditorGUILayout.PropertyField(choice.FindPropertyRelative("moodChange"), GUILayout.MaxWidth(250));
						GUI.enabled = true;
					}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					toggle = choice.FindPropertyRelative("overrideNextStep");
					EditorGUILayout.PropertyField(toggle, GUILayout.MaxWidth(200));
					if (toggle.boolValue)
						EditorGUILayout.PropertyField(choice.FindPropertyRelative("nextStep"), GUILayout.MaxWidth(250));
					else
					{
						GUI.enabled = false;
						EditorGUILayout.PropertyField(choice.FindPropertyRelative("nextStep"), GUILayout.MaxWidth(250));
						GUI.enabled = true;
					}
				EditorGUILayout.EndHorizontal();
			}
			break;

		case StepType.Animation:
			property = step.FindPropertyRelative("animations");
			elementProperty = property;

			elementList.drawElementCallback = DrawAnimationElement;
			elementList.onAddCallback = AddAnimationElement;
			elementList.serializedProperty = property;

			EditorGUILayout.Space(); EditorGUILayout.Space();
			elementList.DoLayoutList();
			NextStepProperty(step);

			if (elementList.count > 0)
			{
				SerializedProperty animation = property.arraySize > selectedElement ? property.GetArrayElementAtIndex(selectedElement) : property.GetArrayElementAtIndex(property.arraySize - 1);

				EditorGUILayout.LabelField("Animation" + (selectedElement + 1), labelStyle);
				EditorGUILayout.PropertyField(animation.FindPropertyRelative("mainCharacter"));
				EditorGUILayout.PropertyField(animation.FindPropertyRelative("animationClip"));

				SerializedProperty speed = animation.FindPropertyRelative("playbackSpeed");
				EditorGUILayout.PropertyField(speed, GUILayout.MaxWidth(250));
				speed.floatValue = Mathf.Max(0, speed.floatValue);
				
				EditorGUILayout.PropertyField(animation.FindPropertyRelative("flip"));

				if (animation.FindPropertyRelative("mainCharacter").boolValue)
				{
					GUI.enabled = false;
					EditorGUILayout.PropertyField(animation.FindPropertyRelative("position"));
					GUI.enabled = true;
				}
				else
					EditorGUILayout.PropertyField(animation.FindPropertyRelative("position"));

				EditorGUILayout.Space();
				SerializedProperty loop = animation.FindPropertyRelative("loop");
				SerializedProperty motion = animation.FindPropertyRelative("applyMotion");

				EditorGUILayout.BeginHorizontal();
					if(motion.boolValue)
					{
						GUI.enabled = false;
						EditorGUILayout.PropertyField(loop);
						GUI.enabled = true;
					}
					else
						EditorGUILayout.PropertyField(loop);				

					EditorGUILayout.PropertyField(motion);
				EditorGUILayout.EndHorizontal();
				
				if(motion.boolValue)
				{
					loop.boolValue = false;					
					EditorGUILayout.PropertyField(animation.FindPropertyRelative("destination"));
					SerializedProperty travelTime = animation.FindPropertyRelative("travelTime");
					EditorGUILayout.PropertyField(travelTime, GUILayout.MaxWidth(250));
					travelTime.floatValue = Mathf.Max(0, travelTime.floatValue);
					EditorGUILayout.PropertyField(animation.FindPropertyRelative("waitForEnd"));
				}
				else if (loop.boolValue)
				{
					motion.boolValue = false;
					SerializedProperty stopEnd = animation.FindPropertyRelative("stopAtEnd");
					GUILayout.BeginHorizontal();
						EditorGUILayout.PropertyField(stopEnd, GUILayout.MaxWidth(200));
						if (stopEnd.boolValue)
						{
							GUI.enabled = false;
							EditorGUILayout.PropertyField(animation.FindPropertyRelative("stopAtStep"), GUILayout.MaxWidth(250));
							GUI.enabled = true;
						}
						else
							EditorGUILayout.PropertyField(animation.FindPropertyRelative("stopAtStep"), GUILayout.MaxWidth(250));
					GUILayout.EndHorizontal();
				}
				else
					EditorGUILayout.PropertyField(animation.FindPropertyRelative("waitForEnd"));
			}
			break;

		case StepType.Sound:
			property = step.FindPropertyRelative("sounds");
			elementProperty = property;

			elementList.drawElementCallback = DrawSoundElement;
			elementList.onAddCallback = AddSoundElement;
			elementList.serializedProperty = property;

			EditorGUILayout.Space(); EditorGUILayout.Space();
			elementList.DoLayoutList();
			NextStepProperty(step);

			if(elementList.count > 0)
			{
				SerializedProperty sound = property.arraySize > selectedElement ? property.GetArrayElementAtIndex(selectedElement) : property.GetArrayElementAtIndex(property.arraySize - 1);

				EditorGUILayout.LabelField("Sound " + (selectedElement + 1), labelStyle);
				EditorGUILayout.PropertyField(sound.FindPropertyRelative("audioClip"));

				SerializedProperty volume = sound.FindPropertyRelative("volume");
				EditorGUILayout.PropertyField(sound.FindPropertyRelative("volume"), GUILayout.MaxWidth(250));
				volume.floatValue = Mathf.Clamp(volume.floatValue, 0f, 1f);

				SerializedProperty loop = sound.FindPropertyRelative("loop");
				EditorGUILayout.PropertyField(loop);
				if (loop.boolValue)
				{
					SerializedProperty stopEnd = sound.FindPropertyRelative("stopAtEnd");
					GUILayout.BeginHorizontal();
						EditorGUILayout.PropertyField(stopEnd, GUILayout.MaxWidth(200));
						if (stopEnd.boolValue)
						{
							GUI.enabled = false;
							EditorGUILayout.PropertyField(sound.FindPropertyRelative("stopAtStep"), GUILayout.MaxWidth(250));
							GUI.enabled = true;
						}
						else
							EditorGUILayout.PropertyField(sound.FindPropertyRelative("stopAtStep"), GUILayout.MaxWidth(250));
					GUILayout.EndHorizontal();
				}
				else
					EditorGUILayout.PropertyField(sound.FindPropertyRelative("waitForEnd"));
			}
			break;

		case StepType.ChangeScene:
			property = step.FindPropertyRelative("changeScene");
			property.isExpanded = true;

			EditorGUILayout.Space(); EditorGUILayout.Space();
			EditorGUILayout.PropertyField(property);
			NextStepProperty(step);
			break;

		case StepType.Custom:
			property = step.FindPropertyRelative("customStep");

			EditorGUILayout.Space(); EditorGUILayout.Space();
			EditorGUILayout.PropertyField(property);
			NextStepProperty(step);	
			break;

		default:
			Debug.Log("Unknown StepType, step " + selectedStep, serializedObject.targetObject);
			break;
		}
	}

	void NextStepProperty(SerializedProperty step)
	{
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		SerializedProperty toggle = step.FindPropertyRelative("overrideNextStep");
		EditorGUILayout.PropertyField(toggle);
		if (toggle.boolValue)
			EditorGUILayout.PropertyField(step.FindPropertyRelative("nextStep"), GUILayout.MaxWidth(250));
		else
		{
			GUI.enabled = false;
			EditorGUILayout.PropertyField(step.FindPropertyRelative("nextStep"), GUILayout.MaxWidth(250));
			GUI.enabled = true;
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
	}


	void DrawStepsHeader(Rect rect)
	{
		EditorGUI.LabelField(rect, serializedObject.context.name, labelStyle);
	}

	void DrawStepElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		SerializedProperty element = stepList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("stepType");
		EditorGUI.PropertyField(rect, element, new GUIContent("Step " + (index + 1)));

		if (isFocused)
		{
			if(selectedStep != index)
				elementList = new ReorderableList(serializedObject, null, true, false, true, true);
			selectedStep = index;
		}			
	}

	void AddStep(ReorderableList list)
	{
		// Increasing the array size adds an element, we can then assign default values to properties
		int newLast = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;

		SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(newLast);
		element.FindPropertyRelative("stepType").enumValueIndex = 0;
		element.FindPropertyRelative("overrideNextStep").boolValue = false;
		element.FindPropertyRelative("nextStep").intValue = 0;

		element.FindPropertyRelative("dialogues").arraySize = 0;
		element.FindPropertyRelative("choices").arraySize = 0;
		element.FindPropertyRelative("animations").arraySize = 0;
		element.FindPropertyRelative("sounds").arraySize = 0;
		element.FindPropertyRelative("customStep").objectReferenceValue = null;

		SerializedProperty changeScene = element.FindPropertyRelative("changeScene");
		changeScene.FindPropertyRelative("sceneIndex").intValue = 2;
		changeScene.FindPropertyRelative("spawnIndex").intValue = 1;
	}

	void OnStepSelect(ReorderableList list)
	{
		drawElement = true;
		//selectedStep = list.index;	explore list.index in the future
	}

	void OnStepsChanged(ReorderableList list)
	{
		drawElement = false;
		//selectedStep = list.index;	explore list.index in the future

		// ReorderableList keeps information of what it contained at the last frame, so we make a new one
		// when 'steps' changes otherwise it'd be confused by the different set of properties
		elementList = new ReorderableList(serializedObject, null, true, false, true, true);
	}


	void DrawDialogueElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		// Create preview string
		string preview = elementProperty.GetArrayElementAtIndex(index).FindPropertyRelative("text").stringValue;
		if (preview == string.Empty)
			preview = "--";

		EditorGUI.LabelField(rect, "Dialogue " + (index + 1), preview);
		if (isFocused)
			selectedElement = index;
	}

	void DrawChoiceElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		string preview = elementProperty.GetArrayElementAtIndex(index).FindPropertyRelative("choiceText").stringValue;
		if (preview == string.Empty)
			preview = "--";

		EditorGUI.LabelField(rect, "Choice " + (index + 1), preview);
		if (isFocused)
			selectedElement = index;
	}

	void DrawAnimationElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		string preview;
		AnimationClip anim = (AnimationClip)elementProperty.GetArrayElementAtIndex(index).FindPropertyRelative("animationClip").objectReferenceValue;
		if (anim)
			preview = anim.name;
		else
			preview = "--";

		EditorGUI.LabelField(rect, "Animation " + (index + 1), preview);
		if (isFocused)
			selectedElement = index;
	}

	void DrawSoundElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		string preview;
		AudioClip clip = (AudioClip)elementProperty.GetArrayElementAtIndex(index).FindPropertyRelative("audioClip").objectReferenceValue;
		if (clip)
			preview = clip.name;
		else
			preview = "--";

		EditorGUI.LabelField(rect, "Sound " + (index + 1), preview);
		if (isFocused)
			selectedElement = index;
	}

	void AddDialogueElement(ReorderableList list)
	{
		int newLast = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;

		SerializedProperty dialogue = list.serializedProperty.GetArrayElementAtIndex(newLast);
		dialogue.FindPropertyRelative("text").stringValue = string.Empty;
		dialogue.FindPropertyRelative("italic").boolValue = false;
	}

	void AddChoiceElement(ReorderableList list)
	{
		int newLast = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;

		SerializedProperty choice = list.serializedProperty.GetArrayElementAtIndex(newLast);
		choice.FindPropertyRelative("choiceText").stringValue = string.Empty;
		choice.FindPropertyRelative("minMood").intValue = 0;
		choice.FindPropertyRelative("maxMood").intValue = 100;
		choice.FindPropertyRelative("overrideNextStep").boolValue = false;
		choice.FindPropertyRelative("nextStep").intValue = 0;
		choice.FindPropertyRelative("overrideMoodChange").boolValue = false;
		choice.FindPropertyRelative("moodChange").intValue = 0;
	}

	void AddAnimationElement(ReorderableList list)
	{
		int newLast = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;

		SerializedProperty animation = list.serializedProperty.GetArrayElementAtIndex(newLast);
		animation.FindPropertyRelative("mainCharacter").boolValue = false;
		animation.FindPropertyRelative("animationClip").objectReferenceValue = null;
		//animation.FindPropertyRelative("childTransformIndex").intValue = 0;
		animation.FindPropertyRelative("position").vector3Value = Vector3.zero;
		animation.FindPropertyRelative("flip").boolValue = false;
		animation.FindPropertyRelative("applyMotion").boolValue = false;
		animation.FindPropertyRelative("destination").vector3Value = Vector3.zero;
		animation.FindPropertyRelative("travelTime").floatValue = 1f;
		animation.FindPropertyRelative("loop").boolValue = false;
		animation.FindPropertyRelative("stopAtEnd").boolValue = true;
		animation.FindPropertyRelative("stopAtStep").intValue = 0;
		animation.FindPropertyRelative("playbackSpeed").floatValue = 1f;
		animation.FindPropertyRelative("waitForEnd").boolValue = true;
	}

	void AddSoundElement(ReorderableList list)
	{
		int newLast = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;

		SerializedProperty sound = list.serializedProperty.GetArrayElementAtIndex(newLast);
		sound.FindPropertyRelative("audioClip").objectReferenceValue = null;
		sound.FindPropertyRelative("volume").floatValue = 1f;
		sound.FindPropertyRelative("loop").boolValue = false;
		sound.FindPropertyRelative("stopAtEnd").boolValue = true;
		sound.FindPropertyRelative("stopAtStep").intValue = 0;
		sound.FindPropertyRelative("waitForEnd").boolValue = false;
	}
}


[CustomEditor(typeof(Event))]
public class EventInspector : Editor
{
	public override void OnInspectorGUI()
	{
		GUI.enabled = false;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
		GUI.enabled = true;

		EditorGUILayout.Space();
		if (GUILayout.Button("Edit"))
			EventEditor.OpenWindow(target as Event);
	}
}
