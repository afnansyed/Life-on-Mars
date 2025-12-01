using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Airlock : MonoBehaviour
{
    public Transform Outdoor;
    public Transform Indoor;
    public ParticleSystem[] pressurizeEffects;
    public TMP_Text oxygenText;
    public Image oxygenFlash;

    private float ChangeDistance = 3f;
    private float DoorSpeed = 2f;
    private float AnimationTime = 5f;
    private float DelayTime = 2f;
    private float OxygenTick = 2f;
    private int Oxygen = 100;

    private Vector3 OutDoorOpenPos;
    private Vector3 InDoorOpenPos;
    private Vector3 OutDoorClosedPos;
    private Vector3 InDoorClosedPos;

    private bool pressurizing = false;
    private bool isOpen = true;

    void Start()
    {
        OutDoorOpenPos = Outdoor.position;
        InDoorClosedPos = Indoor.position;

        OutDoorClosedPos = OutDoorOpenPos + Vector3.up * ChangeDistance;
        InDoorOpenPos = InDoorClosedPos - Vector3.up * ChangeDistance;

        StartCoroutine(OxygenRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
    Debug.Log("Tag: " + other.tag);
        if (!pressurizing && other.CompareTag("Player"))
        {
            pressurizing = true;
            StartCoroutine(AirlockSequence());
        }
    }

    private IEnumerator AirlockSequence()
    {
        Transform doorToClose = isOpen ? Outdoor : Indoor;
        Vector3 closeTarget = isOpen ? OutDoorClosedPos : InDoorClosedPos;

        Transform doorToOpen = isOpen ? Indoor : Outdoor;
        Vector3 openTarget = isOpen ? InDoorOpenPos : OutDoorOpenPos;

        yield return StartCoroutine(MoveDoor(doorToClose, closeTarget));
        yield return new WaitForSeconds(DelayTime);

        foreach(var ps in pressurizeEffects)
            ps.Play();

        yield return new WaitForSeconds(AnimationTime);            

        foreach(var ps in pressurizeEffects)
            ps.Stop();
        yield return new WaitForSeconds(DelayTime);

        yield return StartCoroutine(MoveDoor(doorToOpen, openTarget));

        isOpen = !isOpen;

        pressurizing = false;
    }

    private IEnumerator MoveDoor(Transform door, Vector3 targetPos)
    {
        while (Vector3.Distance(door.position, targetPos) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPos, DoorSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator OxygenRoutine()
    {
        while (true)
        {
            if (isOpen)
            {
                if (Oxygen > 0)
                    Oxygen -= 1;
                oxygenText.text = "Oxygen: " + Oxygen.ToString();
                StartCoroutine(FlashOxygenDepleting());
                yield return new WaitForSeconds(OxygenTick);
            }
            else
            {
                if (Oxygen < 100)
                {
                    Oxygen += 5;
                    if (Oxygen > 100)
                        Oxygen = 100;
                    oxygenText.text = "Oxygen: " + Oxygen.ToString();
                    if(Oxygen >= 20)
                        StartCoroutine(FlashOxygenIncrease());
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator FlashOxygenIncrease()
    {
        Color noColor = new Color(1f, 1f, 1f, 0f);
        oxygenFlash.color = noColor;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            Color c = oxygenFlash.color;
            c.a = Mathf.Lerp(0f, 0.2f, t);
            oxygenFlash.color = c;
            yield return null;
        }
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            Color c = oxygenFlash.color;
            c.a = Mathf.Lerp(0.2f, 0f, t);
            oxygenFlash.color = c;
            yield return null;
        }
    }

    private IEnumerator FlashOxygenDepleting()
    {
        Color redColor = new Color(1f, 0f, 0f, 0f);
        oxygenFlash.color = redColor;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            Color c = oxygenFlash.color;
            c.a = Mathf.Lerp(0f, 1f, t);
            oxygenFlash.color = c;
            yield return null;
        }
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            Color c = oxygenFlash.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            oxygenFlash.color = c;
            yield return null;
        }
    }
}
