#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System

open Fake.Core
open Fake.DotNet
open Fake.IO

let clientPath = Path.getFullName "./src/Client"
let deployDir = Path.getFullName "./deploy"

let platformTool tool winTool =
    let tool = if Environment.isUnix then tool else winTool
    match ProcessUtils.tryFindFileOnPath tool with
    | Some t -> t
    | _ ->
        let errorMsg =
            tool + " was not found in path. " +
            "Please install it and make sure it's available from your path. " +
            "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
        failwith errorMsg

let nodeTool = platformTool "node" "node.exe"
let npmTool = platformTool "npm" "npm.cmd"
let npxTool = platformTool "npx" "npx.cmd"

let runTool cmd args workingDir =
    let arguments = args |> String.split ' ' |> Arguments.OfArgs
    Command.RawCommand (cmd, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let openBrowser url =
    //https://github.com/dotnet/corefx/issues/10361
    Command.ShellCommand url
    |> CreateProcess.fromCommand
    |> CreateProcess.ensureExitCodeWithMessage "opening browser failed"
    |> Proc.run
    |> ignore

// Targets

Target.create "Clean" (fun _ ->
    Shell.cleanDirs [deployDir]
)

Target.create "InstallClient" (fun _ ->
    printfn "Node version:"
    runTool nodeTool "--version" __SOURCE_DIRECTORY__
    printfn "Npm version:"
    runTool npmTool "--version"  __SOURCE_DIRECTORY__
    runTool npmTool "install" __SOURCE_DIRECTORY__
    runDotNet "restore" clientPath
)

Target.create "Build" (fun _ ->
    runTool npxTool "webpack-cli --config webpack.config.js -p" clientPath)

Target.create "Run" (fun _ ->
    let client = async {
        runTool npxTool "webpack-dev-server --config webpack.config.js" clientPath
    }
    let browser = async {
        do! Async.Sleep 5000
        openBrowser "http://localhost:8080"
    }

    let vsCodeSession = Environment.hasEnvironVar "vsCodeSession"

    let tasks =
        [ yield client
          if not vsCodeSession then yield browser ]

    tasks
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

// Build order

open Fake.Core.TargetOperators

"Clean"
    ==> "InstallClient"
    ==> "Build"

"Clean"
    ==> "InstallClient"
    ==> "Run"

Target.runOrDefaultWithArguments "Build"
