using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class UIDocPauseMenu: BaseVisualElement
{
    public CustomButtonsContainer MainMenuButtonsContainer { get; private set; }

    [Preserve]
    public new class UxmlFactory : UxmlFactory<UIDocPauseMenu>
    {
    }

    public UIDocPauseMenu()
    {
        this.LoadStyleSheet(StyleSheetsNames.GlobalStyle);
        this.LoadStyleSheet(StyleSheetsNames.UIDocPauseMenu);

        AddToClassList("mainMenu");

        this.AddNewElement<VisualElement>("MainMenuButtonsVisualElement", "mainMenuBodyContainer")
                                     .Add(BuildMenuButtonsContainer());
    }
    

    private CustomButtonsContainer BuildMenuButtonsContainer()
    {
        var buttons = new List<CustomButton>
        {
            new ("Resume Game", UIMenuEnum.Resume),
            new ("Settings", UIMenuEnum.Options),
            new ("Exit Game", UIMenuEnum.Exit)
        };

        MainMenuButtonsContainer = new CustomButtonsContainer(buttons, CustomButtonsContainer.Orientation.Vertical, true);
        return MainMenuButtonsContainer;
    }
}