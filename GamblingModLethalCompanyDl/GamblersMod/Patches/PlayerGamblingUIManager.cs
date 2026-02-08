using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GamblersMod.Patches
{
	// Token: 0x0200000A RID: 10
	public class PlayerGamblingUIManager : NetworkBehaviour
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00003054 File Offset: 0x00001254
		private void Awake()
		{
			this.gamblingMachineInteractionTextCanvasObject = new GameObject();
			this.gamblingMachineInteractionTextCanvasObject.transform.parent = base.transform;
			this.interactionName = "gamblingMachine";
			this.gamblingMachineInteractionTextCanvasObject.name = this.interactionName + "InteractionTextCanvasObject";
			this.gamblingMachineInteractionTextCanvasObject.AddComponent<Canvas>();
			this.gamblingMachineInteractionTextCanvasObject.SetActive(false);
			this.gamblingMachineInteractionTextCanvas = this.gamblingMachineInteractionTextCanvasObject.GetComponent<Canvas>();
			this.gamblingMachineInteractionTextCanvas.renderMode = 0;
			this.gamblingMachineInteractionTextCanvasObject.AddComponent<CanvasScaler>();
			this.gamblingMachineInteractionTextCanvasObject.AddComponent<GraphicRaycaster>();
			this.gamblingMachineInteractionTextObject = new GameObject();
			this.gamblingMachineInteractionTextObject.name = this.interactionName + "InteractionTextObject";
			this.gamblingMachineInteractionTextObject.AddComponent<Text>();
			this.gamblingMachineInteractionTextObject.transform.localPosition = new Vector3(this.gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2f - 20f, this.gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2f - 50f, 0f);
			this.gamblingMachineInteractionText = this.gamblingMachineInteractionTextObject.GetComponent<Text>();
			this.gamblingMachineInteractionText.text = this.interactionText;
			this.gamblingMachineInteractionText.alignment = 4;
			this.gamblingMachineInteractionText.font = Plugin.GamblingFont;
			this.gamblingMachineInteractionText.rectTransform.sizeDelta = new Vector2(500f, 400f);
			this.gamblingMachineInteractionText.fontSize = 26;
			this.gamblingMachineInteractionText.transform.parent = this.gamblingMachineInteractionTextCanvasObject.transform;
			this.gamblingMachineInteractionScrapInfoTextObject = new GameObject();
			this.gamblingMachineInteractionScrapInfoTextObject.name = this.interactionName + "InteractionScrapInfoTextObject";
			this.gamblingMachineInteractionScrapInfoTextObject.AddComponent<Text>();
			this.gamblingMachineInteractionScrapInfoTextObject.transform.localPosition = new Vector3(this.gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2f - 20f, this.gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2f - 100f, 0f);
			this.gamblingMachineInteractionScrapInfoText = this.gamblingMachineInteractionScrapInfoTextObject.GetComponent<Text>();
			this.gamblingMachineInteractionScrapInfoText.text = this.interactionText;
			this.gamblingMachineInteractionScrapInfoText.alignment = 4;
			this.gamblingMachineInteractionScrapInfoText.font = Plugin.GamblingFont;
			this.gamblingMachineInteractionScrapInfoText.rectTransform.sizeDelta = new Vector2(500f, 300f);
			this.gamblingMachineInteractionScrapInfoText.fontSize = 18;
			this.gamblingMachineInteractionScrapInfoText.color = Color.green;
			this.gamblingMachineInteractionScrapInfoText.transform.parent = this.gamblingMachineInteractionTextCanvasObject.transform;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000334D File Offset: 0x0000154D
		public void SetInteractionText(string text)
		{
			this.gamblingMachineInteractionText.text = text;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000335D File Offset: 0x0000155D
		public void SetInteractionSubText(string text)
		{
			this.gamblingMachineInteractionScrapInfoText.text = text;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000336D File Offset: 0x0000156D
		public void ShowInteractionText()
		{
			this.gamblingMachineInteractionTextCanvasObject.SetActive(true);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000337D File Offset: 0x0000157D
		public void HideInteractionText()
		{
			this.gamblingMachineInteractionTextCanvasObject.SetActive(false);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003390 File Offset: 0x00001590
		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002FDA File Offset: 0x000011DA
		protected override void __initializeRpcs()
		{
			base.__initializeRpcs();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000033A6 File Offset: 0x000015A6
		protected internal override string __getTypeName()
		{
			return "PlayerGamblingUIManager";
		}

		// Token: 0x0400001D RID: 29
		private GameObject gamblingMachineInteractionTextCanvasObject;

		// Token: 0x0400001E RID: 30
		private Canvas gamblingMachineInteractionTextCanvas;

		// Token: 0x0400001F RID: 31
		private GameObject gamblingMachineInteractionTextObject;

		// Token: 0x04000020 RID: 32
		private GameObject gamblingMachineInteractionScrapInfoTextObject;

		// Token: 0x04000021 RID: 33
		private Text gamblingMachineInteractionScrapInfoText;

		// Token: 0x04000022 RID: 34
		private Text gamblingMachineInteractionText;

		// Token: 0x04000023 RID: 35
		private string interactionName;

		// Token: 0x04000024 RID: 36
		private string interactionText;
	}
}
