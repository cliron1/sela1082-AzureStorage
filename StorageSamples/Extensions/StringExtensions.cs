public static class StringExtensions {

	public static string PathToUrl(this string path, string root) {
		return path.Replace(root, string.Empty).Replace("\\", "/");
	}

}
