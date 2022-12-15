using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Mode
{
    EASY,
    NORMAL,
    HARD,
    PAUSE
};

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform patternsContainer;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [SerializeField] private Vector3 floorAndCeilingScale;

    [SerializeField] GameObject crystalBackground;
    [SerializeField] GameObject simpleBackground;
    [SerializeField] GameObject tutoPattern;
    [SerializeField] private GameObject flipPatternPrefab;
    [SerializeField] private GameObject jumpPatternPrefab;
    [SerializeField] private GameObject punchPatternPrefab;
    [SerializeField] private GameObject slidePatternPrefab;

    [SerializeField] private GameObject flippedFlipPatternPrefab;
    [SerializeField] private GameObject flippedJumpPatternPrefab;
    [SerializeField] private GameObject flippedPunchPatternPrefab;
    [SerializeField] private GameObject flippedSlidePatternPrefab;

    [SerializeField] GameObject pausePrefab;
    [SerializeField] int nbRepetitions;

    private int numberOfEasyPatterns;
    private int numberOfNormalPatterns;
    private int numberOfHardPatterns;

    private const int flipSize = 16;
    private const int jumpSize = 8;
    private const int punchSize = 4;
    private const int slidesSize = 12;
    private const int pauseSize = 16;

    private int offset = 16;

    private Vector3 floorPos = new Vector3(0f, -8f, 0f);
    private Vector3 ceilingPos = new Vector3(0f, 8f, 0f);

    private void Awake()
    {
        // pause music in the overworld
        FindObjectOfType<SoundManager>().PauseMusic();

        DifficultyCurve.day++;
        numberOfEasyPatterns = (int)DifficultyCurve.GetNumberPatterns().x;
        numberOfNormalPatterns = (int)DifficultyCurve.GetNumberPatterns().y;
        numberOfHardPatterns = (int)DifficultyCurve.GetNumberPatterns().z;

        CreateBackground();

        switch (DifficultyCurve.day)
        {
            case 1:
                InstantiateTuto();
                break;

            case 2:
            case 3:
                for (int i = 0; i < nbRepetitions; i++)
                {
                    InstantiatePatterns(ref offset, numberOfEasyPatterns, false, Mode.EASY);
                    InstantiatePatterns(ref offset, 1, false, Mode.PAUSE);
                    InstantiatePatterns(ref offset, numberOfNormalPatterns, false, Mode.NORMAL);
                    InstantiatePatterns(ref offset, 1, false, Mode.PAUSE);
                    InstantiatePatterns(ref offset, numberOfHardPatterns, false, Mode.HARD);
                    InstantiatePatterns(ref offset, 3, false, Mode.PAUSE);
                }
                break;

            default:
                for (int i = 0; i < nbRepetitions; i++)
                {
                    InstantiatePatterns(ref offset, numberOfEasyPatterns, false, Mode.EASY);
                    InstantiatePatterns(ref offset, 1, false, Mode.PAUSE);
                    InstantiatePatterns(ref offset, numberOfNormalPatterns, false, Mode.NORMAL);
                    InstantiatePatterns(ref offset, 1, false, Mode.PAUSE);
                    InstantiatePatterns(ref offset, numberOfHardPatterns, false, Mode.HARD);
                    InstantiatePatterns(ref offset, 3, false, Mode.PAUSE);
                }
                break;
        }
    }

    private void CreateFloorAndCeiling()
    {
        GameObject floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, patternsContainer);
        GameObject ceiling = Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity, patternsContainer);

        floor.transform.localScale = floorAndCeilingScale;
        ceiling.transform.localScale = floorAndCeilingScale;
    }

    private void InstantiatePatterns(ref int offset, int nbRecursions, bool isFlipped, Mode difficulty)
    {
        if (nbRecursions > 0)
        {
            float random = Random.Range(0f, 1f);

            float flipRange = 0f;
            float slideRange = 0f;
            float jumpRange = 0f;
            float punchRange = 0f;

            switch (difficulty)
            {
                case Mode.EASY:
                    flipRange = 0.45f;
                    slideRange = 0.80f;
                    jumpRange = 0.95f;
                    punchRange = 1f;
                    break;

                case Mode.NORMAL:
                    flipRange = 0.35f;
                    slideRange = 0.60f;
                    jumpRange = 0.85f;
                    punchRange = 1f;
                    break;

                case Mode.HARD:
                    flipRange = 0.25f;
                    slideRange = 0.50f;
                    jumpRange = 0.75f;
                    punchRange = 1f;
                    break;

                case Mode.PAUSE:
                    for (int i = 0; i < nbRecursions; i++)
                    {
                        Instantiate(pausePrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer);
                        offset += pauseSize;
                    }
                    return;
            }

            if (random <= flipRange)
            {
                GameObject flipPattern = isFlipped ? Instantiate(flippedFlipPatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer) :
                Instantiate(flipPatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer);
                float flipRotation = isFlipped ? 180f : 0f;
                flipPattern.transform.Rotate(flipRotation, 0f, 0f);

                isFlipped = !isFlipped;
                nbRecursions--;
                offset += flipSize;
                InstantiatePatterns(ref offset, nbRecursions, isFlipped, difficulty);
            }
            else if (random > flipRange && random <= slideRange)
            {
                GameObject slidePattern = isFlipped ? Instantiate(flippedSlidePatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer) :
                Instantiate(slidePatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer);
                float slideRotation = isFlipped ? 180f : 0f;
                slidePattern.transform.Rotate(slideRotation, 0f, 0f);

                offset += slidesSize;
                nbRecursions--;
                InstantiatePatterns(ref offset, nbRecursions, isFlipped, difficulty);
            }
            else if (random > slideRange && random <= jumpRange)
            {
                GameObject jumpPattern = isFlipped ? Instantiate(flippedJumpPatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer) :
                Instantiate(jumpPatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer);
                float jumpRotation = isFlipped ? 180f : 0f;
                jumpPattern.transform.Rotate(jumpRotation, 0f, 0f);

                offset += jumpSize;
                nbRecursions--;
                InstantiatePatterns(ref offset, nbRecursions, isFlipped, difficulty);
            }
            else if (random > jumpRange && random <= punchRange)
            {
                GameObject punchPattern = isFlipped ? Instantiate(flippedPunchPatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer) :
                 Instantiate(punchPatternPrefab, new Vector3(offset, 0f, 0f), Quaternion.identity, patternsContainer);
                float punchRotation = isFlipped ? 180f : 0f;
                punchPattern.transform.Rotate(punchRotation, 0f, 0f);

                offset += punchSize;
                nbRecursions--;
                InstantiatePatterns(ref offset, nbRecursions, isFlipped, difficulty);
            }
        }
    }

    void InstantiateTuto()
    {
        Instantiate(tutoPattern, Vector3.zero, Quaternion.identity, patternsContainer);
    }

    void CreateBackground()
    {
        int rand = Random.Range(1, 3);

        GameObject background = rand == 1 ? Instantiate(crystalBackground, Vector3.zero, Quaternion.identity, patternsContainer) :
            Instantiate(simpleBackground, Vector3.zero, Quaternion.identity, patternsContainer);
    }
}
