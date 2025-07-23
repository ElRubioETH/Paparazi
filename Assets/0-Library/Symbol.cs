using System.Collections.Generic;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    [System.Serializable]
    public class Symbol
    {
        public string name;       // Tên hình (ví dụ: "Square")
        public Sprite icon;       // Icon để hiển thị
        [HideInInspector] public int number; // Số ngẫu nhiên
    }

    public List<Symbol> symbols = new List<Symbol>();

    void Start()
    {
        AssignUniqueNumbers();
    }

    void AssignUniqueNumbers()
    {
        List<int> availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        foreach (Symbol symbol in symbols)
        {
            int index = Random.Range(0, availableNumbers.Count);
            symbol.number = availableNumbers[index];
            availableNumbers.RemoveAt(index);
        }

        Debug.Log("Assigned numbers:");
        foreach (Symbol s in symbols)
        {
            Debug.Log($"{s.name} = {s.number}");
        }
    }
}
