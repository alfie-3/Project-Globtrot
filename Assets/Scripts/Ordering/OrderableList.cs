using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Orderables", menuName = "Lists/New Orderable List", order = 0)]
public class OrderableList : ScriptableObject
{
    [SerializeField] Vector2Int minMaxRandomPick = new Vector2Int(1, 3);

    public List<Stock_Item> CurrentOrderablesList;

    public Vector2Int MinMaxTime = new Vector2Int(20, 30);

    public List<OrderItem> PickRandom(int count = 0)
    {
        if (count == 0)
        {
            count = Random.Range(minMaxRandomPick.x, minMaxRandomPick.y + 1);
        }

        Shuffle();

        List<OrderItem> randomPickedList = new(count);

        for (int i = 0; i < count; i++)
        {
            randomPickedList.Add(new(CurrentOrderablesList[i].ItemID, CurrentOrderablesList[i].WeightedQuantitySelection.GetRandom(), CurrentOrderablesList[i].TimeContribution));
        }

        return randomPickedList;
    }

    //github.com/jasonmarziani
    void Shuffle()
    {
        // Loops through array
        for (int i = CurrentOrderablesList.Count - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            Stock_Item temp = CurrentOrderablesList[i];

            // Swap the new and old values
            CurrentOrderablesList[i] = CurrentOrderablesList[rnd];
            CurrentOrderablesList[rnd] = temp;
        }
    }
}
