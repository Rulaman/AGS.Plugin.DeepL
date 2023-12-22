namespace AGS.Plugin.DeepL.Desktop
module Program =

    open System
    open AGS.Plugin.DeepL

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platform.Detect)
        app.Run(new MainForm())
        0