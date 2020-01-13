var SharePlugin =
{
    // 指定されたURLを開くJavascript
    OpenNewWindow: function(openUrl)
    {
        // 引数の定義
        var url = Pointer_stringify(openUrl);

        // 名前を指定して新しいウィンドウを開く
        window.open(url, "ShareWindow");
    }
};

mergeInto(LibraryManager.library, SharePlugin);
