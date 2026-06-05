mergeInto(LibraryManager.library, {
	
	ConsoleError: function (strPtr) {
		var str = UTF8ToString(strPtr);
		window.alert(str);
	},
	
	SaveToLocalStorage: function (keyPtr, valuePtr) {
		var key = UTF8ToString(keyPtr);
		var value = UTF8ToString(valuePtr);
		try {
			localStorage.setItem(key, value);
		}
		catch (e)
		{
			console.error("Ошибка сохранения в localStorage: ", e);
		}
	},
	
	LoadFromLocalStorage: function (keyPtr) {
		var key = UTF8ToString(keyPtr);
		var value = localStorage.getItem(key);
		if (value === null)
		{
			return 0;
		}
		var bufferSize = lengthBytesUTF8(value) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(value, buffer, bufferSize);
		return buffer;
	}
});