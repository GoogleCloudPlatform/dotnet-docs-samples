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
        this.View()

    [<HttpPost>]
    member this.Naughty (files:List<IFormFile>) =
        if null = files || 0 = files.Count then
            this.View()
        else
            let file = files.[0]
            let vision = ImageAnnotatorClient.Create()
            let response = vision.DetectSafeSearch(Image.FromStream(file.OpenReadStream()))
            this.View((file, response))

