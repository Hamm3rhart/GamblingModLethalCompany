using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GamblersMod.Patches
{
    public class PlayerGamblingUIManager : NetworkBehaviour
    {
        private GameObject gamblingMachineInteractionTextCanvasObject;
        private Canvas gamblingMachineInteractionTextCanvas;
        private GameObject gamblingMachineInteractionTextObject;
        private GameObject gamblingMachineInteractionScrapInfoTextObject;
        private Text gamblingMachineInteractionScrapInfoText;
        private Text gamblingMachineInteractionText;
        private string interactionName;
        private string interactionText = string.Empty;

        private void Awake()
        {
            gamblingMachineInteractionTextCanvasObject = new GameObject();
            gamblingMachineInteractionTextCanvasObject.transform.parent = transform;

            interactionName = "gamblingMachine";

            gamblingMachineInteractionTextCanvasObject.name = interactionName + "InteractionTextCanvasObject";
            gamblingMachineInteractionTextCanvasObject.AddComponent<Canvas>();
            gamblingMachineInteractionTextCanvasObject.SetActive(false);

            gamblingMachineInteractionTextCanvas = gamblingMachineInteractionTextCanvasObject.GetComponent<Canvas>();
            gamblingMachineInteractionTextCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gamblingMachineInteractionTextCanvasObject.AddComponent<CanvasScaler>();
            gamblingMachineInteractionTextCanvasObject.AddComponent<GraphicRaycaster>();

            gamblingMachineInteractionTextObject = new GameObject();
            gamblingMachineInteractionTextObject.name = interactionName + "InteractionTextObject";
            gamblingMachineInteractionTextObject.AddComponent<Text>();
            gamblingMachineInteractionTextObject.transform.localPosition = new Vector3(gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2f - 20f, gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2f - 50f, 0f);

            gamblingMachineInteractionText = gamblingMachineInteractionTextObject.GetComponent<Text>();
            gamblingMachineInteractionText.text = interactionText;
            gamblingMachineInteractionText.alignment = TextAnchor.MiddleCenter;
            gamblingMachineInteractionText.font = Plugin.GamblingFont;
            gamblingMachineInteractionText.rectTransform.sizeDelta = new Vector2(500f, 400f);
            gamblingMachineInteractionText.fontSize = 26;

            gamblingMachineInteractionText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

            gamblingMachineInteractionScrapInfoTextObject = new GameObject();
            gamblingMachineInteractionScrapInfoTextObject.name = interactionName + "InteractionScrapInfoTextObject";
            gamblingMachineInteractionScrapInfoTextObject.AddComponent<Text>();
            gamblingMachineInteractionScrapInfoTextObject.transform.localPosition = new Vector3(gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2f - 20f, gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2f - 100f, 0f);

            gamblingMachineInteractionScrapInfoText = gamblingMachineInteractionScrapInfoTextObject.GetComponent<Text>();
            gamblingMachineInteractionScrapInfoText.text = interactionText;
            gamblingMachineInteractionScrapInfoText.alignment = TextAnchor.MiddleCenter;
            gamblingMachineInteractionScrapInfoText.font = Plugin.GamblingFont;
            gamblingMachineInteractionScrapInfoText.rectTransform.sizeDelta = new Vector2(500f, 300f);
            gamblingMachineInteractionScrapInfoText.fontSize = 18;
            gamblingMachineInteractionScrapInfoText.color = Color.green;

            gamblingMachineInteractionScrapInfoText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;
        }

        public void SetInteractionText(string text)
        {
            gamblingMachineInteractionText.text = text;
        }

        public void SetInteractionSubText(string text)
        {
            gamblingMachineInteractionScrapInfoText.text = text;
        }

        public void ShowInteractionText()
        {
            gamblingMachineInteractionTextCanvasObject.SetActive(true);
        }

        public void HideInteractionText()
        {
            gamblingMachineInteractionTextCanvasObject.SetActive(false);
        }
    }
}
