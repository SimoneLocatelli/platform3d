using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class UIDocMainMenu : BaseVisualElement
{
    public CustomButtonsContainer MainMenuButtonsContainer { get; private set; }

    [Preserve]
    public new class UxmlFactory : UxmlFactory<UIDocMainMenu>
    {
    }

    public UIDocMainMenu()
    {
        this.LoadStyleSheet(StyleSheetsNames.GlobalStyle);
        this.LoadStyleSheet(StyleSheetsNames.USSMainMenu);

        AddToClassList("mainMenu");
        AddToClassList("backgroundImage");
        AddToClassList("backgroundImageMainMenu");

        this.AddLabel("Mighty Dungeon", "lblTitle", name: "Label Title");

        this.AddNewElement<VisualElement>("MainMenuButtonsVisualElement", "mainMenuBodyContainer")
                                     .Add(BuildMenuButtonsContainer());
    }
    

    private CustomButtonsContainer BuildMenuButtonsContainer()
    {
        var buttons = new List<CustomButton>
        {
            new ("Start Game", UIMenuEnum.StartGame),
            new ("Settings", UIMenuEnum.Options),
            new ("Quit", UIMenuEnum.Exit)
        };

        MainMenuButtonsContainer = new CustomButtonsContainer(buttons, CustomButtonsContainer.Orientation.Vertical, true);
        return MainMenuButtonsContainer;
    }
}