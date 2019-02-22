module Navbar.View

open Fable.FontAwesome
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let navButton classy href faClass txt =
  Control.div []
    [ Button.a 
        [ Button.CustomClass (sprintf "button %s" classy)
          Button.Props [ Href href ] ]
        [ Icon.icon [] [ Fa.i [ faClass ] [] ]
          span [] [ str txt ] ] ]

let navButtons =
  Navbar.Item.div []
    [ Field.div
        [ Field.IsGrouped ]
        [ navButton "twitter" "https://twitter.com/mark_pattison" Fa.Brand.Twitter "Twitter"
          navButton "github" "https://github.com/markpattison/FableElmishRecharts" Fa.Brand.Github "GitHub" ] ]

let root =
  Navbar.navbar [ Navbar.Color IsPrimary ]
    [ Navbar.Brand.div []
        [ Navbar.Item.div []
            [ Heading.h4 [] [ str "FableElmishRecharts" ] ] ]
      Navbar.End.div []
        [ navButtons ] ]
