using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class Inventory : MonoBehaviour
    {
        public List<Iteam> _iteams;
        public GameObject InventoryCanvas;
        public GameObject IconPrefab;

        private void Start()
        {
            _iteams = new List<Iteam>();
        }

        public void AddIteam(Iteam iteam)
        {
            _iteams.Add(iteam);
            UpdateCanvas();
        }

        private void UpdateCanvas()
        {
            for (int i = 0; i < InventoryCanvas.transform.GetChild(0).childCount; i++)
            {
                Destroy(InventoryCanvas.transform.GetChild(0).GetChild(i).gameObject);
            }
            foreach (var iteam in _iteams)
            {
                
                //iteam.transform.position = Vector3.zero;
                iteam.CurInventory = this;
                IconPrefab.GetComponent<IconIteam>()._iteam = iteam;
                IconPrefab.GetComponent<Image>().sprite = iteam.Icon;
                Instantiate(IconPrefab, InventoryCanvas.transform.GetChild(0));
                
            }
        }

        public void RemoveIteam(Iteam iteam)
        {
            _iteams.Remove(iteam);
            UpdateCanvas();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) 
                && (!InventoryCanvas.transform.GetChild(0).gameObject.activeSelf))
            {
                gameObject.GetComponent<PlayerMovement>().enabled = false;
                InventoryCanvas.transform.GetChild(0).gameObject.SetActive(true);
            }

            else if (InventoryCanvas.transform.GetChild(0).gameObject.activeSelf 
                     && (Input.GetKeyDown(KeyCode.I) 
                         || Input.GetKeyDown(KeyCode.Escape)))
            {
                gameObject.GetComponent<PlayerMovement>().enabled = true;
                InventoryCanvas.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}