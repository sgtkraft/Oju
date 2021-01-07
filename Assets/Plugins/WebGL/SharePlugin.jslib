mergeInto(
    LibraryManager.library,
    {
        GetWebUrl: function()
        {
            // WebGL実行中のURLを取得する
            //var url = window.location.href;
            var returnStr = document.referrer;
            var bufferSize = lengthBytesUTF8(returnStr) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(returnStr, buffer, bufferSize);
            return buffer;
        },
        OpenNewWindow: function(openUrl)
        {
            // URLを指定して新しいウィンドウを開く
            window.open(Pointer_stringify(openUrl), "ShareWindow");
        },
    }
);
