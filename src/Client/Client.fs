module Client

open Elmish
open Elmish.React

open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Helpers.ReactGoogleMaps
open Fable.Helpers.ReactGoogleMaps.Props
module P = Fable.Helpers.React.Props

open Fulma
open Fable.Core
open Fable.Import.GoogleMaps

importAll "./sass/main.sass"

let defaultCenter:Fable.Import.GoogleMaps.LatLngLiteral = Fable.Helpers.GoogleMaps.Literal.createLatLng 51.818210 -0.359048
let googleMapsApiKey = FableElmishGoogleMaps.GoogleMapsApiKey

let markers =
    [| (51.81, -0.36); (51.82, -0.36); (51.815, -0.35) |]
    |> Array.mapi (fun i (x, y) ->
        marker [
            MarkerProperties.Key i
            MarkerProperties.Position !^ (Fable.Helpers.GoogleMaps.Literal.createLatLng x y)
            MarkerProperties.Icon "http://www.google-analytics.com/__utm.gif"
            MarkerProperties.Title (sprintf "Point %i" i)
            MarkerProperties.Label 
              (markerLabel [ MarkerLabelProperties.Text "Hello" ] []) ] [])




type View = Page1 | Page2

type Model = { View: View }

type Msg = | ShowPage of View

let init () : Model * Cmd<Msg> =
    { View = Page1 }, []

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | ShowPage page -> { currentModel with View = page }, []

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.Props [ OnClick (fun _ -> ShowPage page |> dispatch) ] ]
      [ str label ]

let menu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "General" ]
      Menu.list []
        [ menuItem "Page 1" Page1 currentPage dispatch
          menuItem "Page 2" Page2 currentPage dispatch ] ]

let navBar =
    div []
      [ Navbar.navbar [ Navbar.Color IsPrimary ]
          [ Navbar.Brand.div []
              [ Navbar.Item.div []
                  [ Heading.h4 [] [ str "FableElmishGoogleMaps" ] ] ] ] ]

let map =
  googleMap
   [ MapProperties.ApiKey googleMapsApiKey
     MapProperties.MapLoadingContainer "maploadercontainer"
     MapProperties.MapContainer "mapcontainer"
     MapProperties.DefaultZoom 9
     MapProperties.DefaultCenter !^ defaultCenter
     MapProperties.Center !^ defaultCenter
     MapProperties.Markers markers ]

let view (model : Model) (dispatch : Msg -> unit) =
  
    let content =
        match model.View with
        | Page1 ->
            div []
              [ div [] [ Heading.h4 [] [ str "Page 1" ] ]
                map ]
        | Page2 ->
            div []
              [ div [] [ Heading.h4 [] [ str "Page 2" ] ]
                 ]
    div []
        [ div
            [ ClassName "navbar-bg" ]
            [ Container.container []
                [ Navbar.View.root ] ]
          Section.section []
            [ Container.container []
                [ Columns.columns []
                    [ Column.column
                        [ Column.Width (Screen.All, Column.Is2) ]
                        [ menu model.View dispatch ]
                      Column.column []
                        [ content ] ] ] ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
|> Program.run
