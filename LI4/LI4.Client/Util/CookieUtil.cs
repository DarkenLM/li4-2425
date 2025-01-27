using Microsoft.JSInterop;

namespace LI4.Client.Util;

public class CookieUtil {
    private readonly IJSRuntime js;
    public CookieUtil(IJSRuntime jSjsRuntime) {
        this.js = js;
    }

    public async Task setCookie(string name, string value) {
        await js.InvokeVoidAsync("createCookie", name, value);
    }

    public async Task<bool> hasCookie(string name) {
        return getCookie(name) != null;
    }

    public async Task<string?> getCookie(string name) {
        string? value = await js.InvokeAsync<string>("readCookie", name);
        return value;
    }
}
