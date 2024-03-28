using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class UIDocConfirmExitPopup : BaseVisualElement
{
    public CustomButtonsContainer ButtonsContainer { get; private set; }

    [Preserve]
    public new class UxmlFactory : UxmlFactory<UIDocConfirmExitPopup>
    {
    }

    public UIDocConfirmExitPopup()
        : base("UIDocConfirmExitPopup", 
               CssClasses.Popup)
    {
        this.LoadStyleSheet(StyleSheetsNames.GlobalStyle);
        this.LoadStyleSheet(StyleSheetsNames.Popups);
         
        this.AddNewElement<VisualElement>("Menu Background", "darkBackground");

        var popupBody = this.AddNewElement<VisualElement>("Popup Body", CssClasses.PopupBody, CssClasses.BackgroundBorderPrimary, CssClasses.Container_Vertical);
        popupBody.AddLabel("Are sure you want to exit the game?", CssClasses.LabelText, name: "Label Popup Text");
        popupBody.Add(BuildMenuButtonsContainer());
    }


    private CustomButtonsContainer BuildMenuButtonsContainer()
    {
        var buttons = new List<CustomButton>
        {
            new ("Exit Game", UIMenuEnum.Yes),
            new ("Cancel", UIMenuEnum.No),
        };

        ButtonsContainer = new CustomButtonsContainer(buttons, CustomButtonsContainer.Orientation.Horizontal, false)
                               .SetArrowSizeToSmall();

        return ButtonsContainer;
    }
}