using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILabelList : MonoBehaviour
{
    // The label prefab that will fill the list
    public GameObject labelPrefab;

    // The sprite that will contain the labels
    public UISprite spriteContainer;

    // The list containing the items
    List<ListItem> list = new List<ListItem>();

    /// <summary>
    /// Adds a new item to the list
    /// </summary>
    public void AddListItem(string _text)
    {
        ListItem item = new ListItem(_text, gameObject);

        if (list.Count <= 0)
            item.UpdateAnchors(true, spriteContainer, spriteContainer);
        else
            item.UpdateAnchors(false, list[(list.Count - 1)].label, spriteContainer);

        list.Add(item);
    }

    /// <summary>
    /// Removes the first list item matching the name and then updates the list
    /// </summary>
    public void RemoveListItem(string _name)
    {
        foreach (ListItem li in list)
        {
            if (li.name == _name)
            {
                list.Remove(li);
                Destroy(li.label.gameObject);
                UpdateList();
                break;
            }
        }
    }

    /// <summary>
    /// Clears all items on the list and destroys referenced labels
    /// </summary>
    public void ClearList()
    {
        foreach (ListItem li in list)
        {
            Destroy(li.label.gameObject);
        }

        list.Clear();
    }

    /// <summary>
    /// Updates the labels anchors and positions on the list
    /// </summary>
    void UpdateList()
    {
        for (int i = 0; i < list.Count; i++)
        {
            // Does the same thing as in AddListItem, but with some clever logic
            list[i].UpdateAnchors(
                (i == 0),
                (i == 0) ? (UIWidget)spriteContainer : (UIWidget)list[(i - 1)].label,
                spriteContainer);
        }
    }

    /// <summary>
    /// The custom class that is stored in the list
    /// </summary>
    class ListItem
    {
        public string name = "";
        public UILabel label;

        public ListItem(string _name, GameObject _parent)
        {
            name = _name;

            GameObject go = (GameObject)Instantiate(Resources.Load("UI/LabelList_Label"), Vector3.zero, Quaternion.identity);
            go.transform.parent = _parent.transform;
            go.transform.localScale = Vector3.one;
            go.name = "ListItem (" + name + ")";

            label = go.GetComponent<UILabel>();
            label.text = name;
        }

        public void UpdateAnchors(bool _isFirstItem, UIWidget _parentItem, UIWidget _spriteContainer)
        {
            // Set X anchors
            label.rightAnchor.target = _spriteContainer.transform;
            label.leftAnchor.target = _spriteContainer.transform;

            // Limit X anchors to the container sprite
            label.rightAnchor.absolute = -10;
            label.leftAnchor.absolute = 10;

            // Set Y anchors
            label.bottomAnchor.target = _parentItem.transform;
            label.topAnchor.target = _parentItem.transform;

            // Set top and bottom anchors to the parent items anchors
            // If its the first element in the list, the parent item is the container
            if (_isFirstItem)
            {
                label.topAnchor.Set(1f, -5f); // Anchor to parent top with offset
                label.bottomAnchor.Set(1f, label.topAnchor.absolute - label.height); // Anchor bottom to top, and move down by the offset and height
            }
            else
            {
                label.topAnchor.Set(0f, 0f); // Anchor to parent bottom
                label.bottomAnchor.absolute = -label.height;
            }

            label.ResetAndUpdateAnchors(); // Required or else the anchors will start falling
        }
    }
}
