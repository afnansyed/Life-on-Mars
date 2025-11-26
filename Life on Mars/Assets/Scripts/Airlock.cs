using UnityEngine;
using System.Collections;

public class Airlock : MonoBehaviour
{
    public Transform Outdoor;
    public Transform Indoor;
    public ParticleSystem[] pressurizeEffects;

    private float ChangeDistance = 3f;
    private float DoorSpeed = 2f;
    private float AnimationTime = 5f;
    private float DelayTime = 2f;

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
}
