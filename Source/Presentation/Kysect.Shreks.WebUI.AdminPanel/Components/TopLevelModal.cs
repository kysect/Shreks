using Blazorise.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Kysect.Shreks.WebUI.AdminPanel.Components;

public class TopLevelModal : Modal
{
    public TopLevelModal()
    {
        Style = "z-index: 999999;";
        BackdropVisible = false;
    }

    [Inject]
    public IJSRuntime? JsRuntime { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        base.BuildRenderTree(__builder);

        if (BackdropVisible)
        {
            __builder.OpenComponent<Blazorise._ModalBackdrop>(15);
            __builder.AddAttribute(16, "CloseActivatorDisabled", true);
            __builder.AddAttribute(17, "Style", "z-index: 999998;");
            __builder.CloseComponent();
        }
    }
}