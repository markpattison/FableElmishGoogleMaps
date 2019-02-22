module Client

open Elmish
open Elmish.React

open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
module P = Fable.Helpers.React.Props

open Fulma
open Fable.Core

importAll "./sass/main.sass"

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
                  [ Heading.h4 [] [ str "FableElmishRecharts" ] ] ] ] ]



let view (model : Model) (dispatch : Msg -> unit) =
  
    let content =
        match model.View with
        | Page1 ->
            div []
              [ div [] [ Heading.h4 [] [ str "Page 1" ] ]
                 ]
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
