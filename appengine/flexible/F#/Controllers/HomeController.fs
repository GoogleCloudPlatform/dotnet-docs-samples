namespace F_.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open Google.Cloud.Vision.V1

type HomeController () =
    inherit Controller()

    member this.Index () =
        this.View()

    member this.About () =
        this.ViewData.["Message"] <- "Your application description page."
        this.View()

    member this.Contact () =
        this.ViewData.["Message"] <- "Your contact page."
        this.View()

    member this.Error () =
        this.View()

    member this.Naughty () =
        this.View(new List<Tuple<IFormFile, AnnotateImageResponse>>())

    [<HttpPost>]
    member this.Naughty (files:List<IFormFile>) =
        let vision = ImageAnnotatorClient.Create()        
        let requests = seq { for file in files do yield AnnotateImageRequest(Image = Image.FromStream(file.OpenReadStream())) }
        let response = vision.BatchAnnotateImages(requests)
        let rr = Seq.zip files response.Responses
        this.View(rr)

