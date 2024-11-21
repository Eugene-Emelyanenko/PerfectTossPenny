using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public BoxData boxData;
    private List<BoxData> boxDataList = new List<BoxData>();
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxDataList = BoxDataManager.LoadBoxData();
        boxData = boxDataList.Find(data => data.isSelected);
    }

    private void Start()
    {
        spriteRenderer.sprite = Resources.Load<Sprite>($"GameBox/{boxData.index}");
    }
}
