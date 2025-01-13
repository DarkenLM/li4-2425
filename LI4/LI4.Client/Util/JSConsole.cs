using Microsoft.JSInterop;

namespace LI4.Client.Util;

/// <summary>
/// Provides a helper to invoke the browser's native console. Requires the script at <code>js/util/interopUtil.js</code> to function.
/// </summary>
public class JSConsole {
    private readonly IJSRuntime JsRuntime;
    public JSConsole(IJSRuntime jSRuntime) {
        this.JsRuntime = jSRuntime;
    }

    public async Task log(params object[] messages) {
        await this.JsRuntime.InvokeVoidAsync("consoleLogHelper", (object)messages);
    }

    public async Task error(params object[] messages) {
        await this.JsRuntime.InvokeVoidAsync("consoleErrorHelper", (object)messages);
    }
}
