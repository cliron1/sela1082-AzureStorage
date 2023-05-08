using Microsoft.AspNetCore.Mvc;
using StorageSamples.Servcies;

namespace StorageSamples.Controllers;

public class GalleryController : Controller {
    private readonly IStorageService service;

    public GalleryController(IStorageProvider storageProvider) {
        service = storageProvider.Service;
    }

    public async Task<IActionResult> Index() {

        var liron = new Person("liron", "Cohen") { Age = 43};
        Person mirit = null;


        var files = await service.GetFiles();
		return View(files);
	}
    public record Person(string FirstName, string LastName) {
        public int Age { get; set; }
    };

	[HttpPost]
	public async Task<IActionResult> Upload(IFormCollection formData) {
        var allowedContentTypes = new List<string> { "image/png", "image/jpeg" };

        foreach(var file in formData.Files) {
            if(!allowedContentTypes.Contains(file.ContentType))
                continue;

            await service.Upload(file);

			// Todo: Itamar: save "fileName" to database

            //await storage.SaveFile(file);
        }

        return RedirectToAction(nameof(Index));
    }
}
