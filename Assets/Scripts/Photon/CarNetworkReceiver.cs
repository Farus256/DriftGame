using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class CarNetworkReceiver : MonoBehaviourPun
{
    private void Awake()
    {
        // Берём InstantiationData
        if(!PhotonNetwork.IsConnected) return;
        if (photonView != null && photonView.InstantiationData != null)
        {
            object[] data = photonView.InstantiationData;

            int carID = (int)data[0];
            string colorHex = (string)data[1];
            string[] partsArray = (string[])data[2];
            
            // Применяем цвет:
            ApplyPaintColor(gameObject, colorHex);

            // Применяем тюнинг:
            var partsList = new List<string>(partsArray);
            ApplyExtraParts(gameObject, partsList);
        }
        else
        {
            Debug.LogWarning("[CarNetworkReceiver] Нет InstantiationData — не можем применить цвет/тюнинг.");
        }
    }

    private void ApplyPaintColor(GameObject car, string colorHex)
    {
        if (ColorUtility.TryParseHtmlString(colorHex, out Color newColor))
        {
            Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.color = newColor;
                }
            }
        }
        else
        {
            Debug.LogWarning($"[CarNetworkReceiver] Некорректный цвет: {colorHex}");
        }
    }

    private void ApplyExtraParts(GameObject car, List<string> activeParts)
    {
        string[] possibleParts = { "Spoiler", "SideSkirts" };
        foreach (string partName in possibleParts)
        {
            Transform partTransform = car.transform.Find(partName);
            if (partTransform)
            {
                bool isActive = activeParts.Contains(partName);
                partTransform.gameObject.SetActive(isActive);
            }
            else
            {
                Debug.LogWarning($"[CarNetworkReceiver] Не найдено дополнение: {partName} на автомобиле {car.name}");
            }
        }
    }
}
