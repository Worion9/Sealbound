using System.Collections;
using UnityEngine;
using static Card;

public class DropZone : MonoBehaviour
{
    public bool spellDropZone;
    public int associatedListIndex;
    public int slotIndex;
    public DropZone myCardContainer;
    [SerializeField] private Transform myRowTriggers;
    [SerializeField] private GameObject myLastTrigger;
    public int cardCount;
    public bool enemyRow;
    public ParentRowListEnum parentRowListEnum;

    public void AjustRowTriggers()
    {
        StartCoroutine(DelayedFullSync());
    }

    private IEnumerator DelayedFullSync()
    {
        yield return null;

        if (slotIndex != -1) yield break;
        if (cardCount % 2 == 1)
        {
            myLastTrigger.SetActive(false);
            myRowTriggers.localPosition = Vector3.zero;
        }
        else
        {
            myLastTrigger.SetActive(true);
            myRowTriggers.localPosition = new Vector3(-47.5f, 0f, 0f);
        }
    }
}