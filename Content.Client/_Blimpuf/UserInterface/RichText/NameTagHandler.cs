using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Utility;

namespace Content.Client._Blimpuf.UserInterface.RichText;

/// <summary>
/// Displays each [name] field as a button that fills in the clicking character's name.
/// </summary>
public sealed class NameTagHandler : IMarkupTagHandler
{
    private const string ButtonName = "paper_name";

    public string Name => "name";

    /// <summary>
    /// Used to ensure the button height matches the paper's text height.
    /// </summary>
    public static float FontLineHeight { get; set; } = 16.0f;

    /// <summary>
    /// Creates a button that asks its containing PaperWindow to fill the corresponding [name] field.
    /// </summary>
    public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        var button = new Button
        {
            Name = ButtonName,
            Text = Loc.GetString("paper-name-button"),
            MinSize = new Vector2(80, FontLineHeight + 4),
            MaxSize = new Vector2(80, FontLineHeight + 4),
            Margin = new Thickness(1, 2, 1, 2),
            StyleClasses = { "ButtonSquare" },
            TextAlign = Label.AlignMode.Center,
        };

        button.OnPressed += _ =>
        {
            if (button.Parent is not { } container)
                return;

            var parent = container;
            while (parent is not null and not PaperWindow)
                parent = parent.Parent;

            if (parent is not PaperWindow window)
                return;

            // Double check attached buttons because the renderer can also create temporary controls while parsing.
            var fieldIndex = 0;
            foreach (var child in container.Children)
            {
                if (child == button)
                    break;
                if (child is Button { Name: ButtonName })
                    fieldIndex++;
            }

            window.FillNameField(fieldIndex);
        };

        control = button;
        return true;
    }
}
