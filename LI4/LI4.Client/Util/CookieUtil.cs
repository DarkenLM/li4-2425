using Microsoft.JSInterop;

namespace LI4.Client.Util;

public class CookieUtil {
    private readonly IJSRuntime js;
    public CookieUtil(IJSRuntime jSjsRuntime) {
        this.js = js;
    }

    public async Task setCookie(string name, string value) {
        await js.InvokeVoidAsync("createCookie", name, value, 7);
    }

    public async Task<bool> hasCookie(string name) {
        return getCookie(name) != null;
    }

    public async Task<string?> getCookie(string name) {
        string? value = await js.InvokeAsync<string>("readCookie", name);
        return value;
    }

    public static async Task<string> getUserAuth(IJSRuntime js) {
        try {
            var cookieValue = await js.InvokeAsync<string>("readCookie", "userAuth");
            if (cookieValue is null) throw new Exception("Unable to read cookie value.");

            //int userId = Int32.Parse(cookieValue);
            return cookieValue;
        } catch (Exception e) {
            throw new Exception("Unable to get user auth: " + e);
        }
    }
}
