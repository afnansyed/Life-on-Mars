using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SeedObject : MonoBehaviour
{
    [Header("Seed Data")]
    public TreeSeed seedType;
    public LayerMask groundLayer;

    [Header("Plant Prefab")]
    public GameObject plantPrefab;

    private bool wasThrown;

    public void SetThrown()
    {
        wasThrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && wasThrown)
        {
            PlantSeed(transform.position - new Vector3(0, 0.05f, 0), collision.contacts[0].normal);
        }
    }

    private void PlantSeed(Vector3 position, Vector3 normal)
    {
        if (seedType == null)
        {
            Debug.LogError("Cannot plant: seedType is null!");
            return;
        }

        GameObject plant;

        // use prefab if provided
        if (plantPrefab != null)
        {
            plant = Instantiate(plantPrefab, position, Quaternion.FromToRotation(Vector3.up, normal));
        }
        else
        {
            plant = new GameObject($"Plant_{seedType.name}");
            plant.transform.position = position;
            plant.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

            plant.AddComponent<MeshFilter>();
            plant.AddComponent<MeshRenderer>();
        }

        PlantGrowth growth = plant.GetComponent<PlantGrowth>();
        if (growth == null)
        {
            growth = plant.AddComponent<PlantGrowth>();
        }

        growth.treeSeed = seedType;

        Debug.Log($"Planted {seedType.name} at {position}");

        // destroy the seed object (since planted)
        Destroy(gameObject);
    }
}