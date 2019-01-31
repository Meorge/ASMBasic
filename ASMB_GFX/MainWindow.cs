using System;
using System.Timers;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
using Gtk;
using Gdk;
using System.Threading;
using System.Threading.Tasks;
//using Cairo;

public partial class MainWindow : Gtk.Window
{

    public int[,] pixelValues = new int[32,32];
    int sizeMult = 12;

    int currentColor = 1;

    //Cairo.Context cairoContext;

    System.Timers.Timer timer;

    Gtk.Image img;

    ASMBASIC.ASMBASICInterpreter interpreter;

    Thread graphicsThread;
    Thread codeThread;

    string codePath = "movingsq.asmb";


    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        ClearScreen();

        
        pixelValues.Initialize();
        //Resizable = false;
        Resize(pixelValues.GetLength(0) * sizeMult, pixelValues.GetLength(1) * sizeMult);

        //cairoContext = CairoHelper.Create(base.GdkWindow);


        //System.Threading.Tasks.Task graphicsUpdateTask = new System.Threading.Tasks.Task(StartTimer);

        Task<int> task = new Task<int>(StartTimer);
        task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
        task.Start();

        try {
            task.Wait();
        } catch (AggregateException ex) {
            Console.WriteLine(ex);
        }
        //graphicsUpdateTask.ContinueWith(ExceptionHandler, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);


        //ThreadStart graphicsThreadStart = new ThreadStart(StartTimer);

        //graphicsThread = new Thread(graphicsThreadStart);
        //graphicsThread.Start();
        //StartTimer();

        ThreadStart codeThreadStart = new ThreadStart(StartCodeExec);
        codeThread = new Thread(codeThreadStart);
        //codeThread.Start();
        StartCodeExec();


        //Pixmap pixmap = new Pixmap(Gdk.Screen.Default.RootWindow, 120, 120);
        //Gdk.GC gC = new Gdk.GC((Drawable)base.GdkWindow);

        //pixmap.DrawRectangle(gC, true, 0,0,120,120);

        img = new Gtk.Image();

        base.Add(img);

        KeyPressEvent += KeyPress;

       



        //args.RetVal = true;
    }

    static void ExceptionHandler(Task<int> task)
    {
        var exception = task.Exception;
        Console.WriteLine(exception);
    }

    [GLib.ConnectBefore]
    public void KeyPress(object sender, KeyPressEventArgs args) {
        System.Console.WriteLine("Key press");
        if (args.Event.Key == Gdk.Key.w) {
            interpreter.argsIn[0] = "10";
            System.Console.WriteLine("Move up!");
        } else {
            interpreter.argsIn[0] = "00";
        }

        if (args.Event.Key == Gdk.Key.a)
        {
            interpreter.argsIn[1] = "10";
        }
        else
        {
            interpreter.argsIn[1] = "00";
        }

        if (args.Event.Key == Gdk.Key.s)
        {
            interpreter.argsIn[2] = "10";
        }
        else
        {
            interpreter.argsIn[2] = "00";
        }

        if (args.Event.Key == Gdk.Key.d)
        {
            interpreter.argsIn[3] = "10";
        }
        else
        {
            interpreter.argsIn[3] = "00";
        }

        //interpreter.EvaluateCode(interpreter.gotos[255], false);
        //draw(null, null);
    }

    public void ClearScreen() {
        for (int x = 0; x < pixelValues.GetLength(0); x++)
        {
            for (int y = 0; y < pixelValues.GetLength(1); y++)
            {
                pixelValues[x, y] = 0;
            }
        }
    }

    void StartCodeExec() {
        interpreter = new ASMBASIC.ASMBASICInterpreter(codePath, this);
        interpreter.argsIn = new string[4];
        for (int i = 0; i < interpreter.argsIn.Length; i++) {
            interpreter.argsIn[i] = "00";
        }
        //interpreter.Start(new string[] { "examplegfx.asmb" });
    }

    int StartTimer() {
        timer = new System.Timers.Timer(90);

        timer.AutoReset = false;
        timer.Enabled = true;
        timer.Start();
        timer.Elapsed += draw;
        return 1;

        //while (true) {
        //}
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected override bool OnExposeEvent(EventExpose evnt)
    {
        bool ok = base.OnExposeEvent(evnt);
        return ok;
    }

    public void DoneInterpreting() {
        timer.Start();
    }

    void draw(object source, ElapsedEventArgs e)
    {


        System.Console.WriteLine("Evaluating code");

        System.Console.WriteLine("Drawing");


        int colOffset = 0;
        using (Cairo.Context cairoContext = CairoHelper.Create(base.GdkWindow))
        {
            for (int x = 0; x < pixelValues.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < pixelValues.GetLength(1) - 1; y++)
                {
                    //pixelValues[x, y] = colOffset;
                    //colOffset++;

                    if (colOffset > 8) {
                        colOffset = 0;
                    }
                    //System.Console.Write(pixelValues[x, y]);
                    if (pixelValues[x, y] != 0) { System.Console.WriteLine("Color ID " + pixelValues[x, y].ToString() + " at point (" + x.ToString() + "," + y.ToString() + ")"); }

                    
                    cairoContext.MoveTo(x * sizeMult, y * sizeMult);
                    cairoContext.SetSourceColor(GetColorVal(pixelValues[x, y]));
                    cairoContext.Rectangle(x * sizeMult, y * sizeMult, sizeMult, sizeMult);
                    cairoContext.Fill();


                    //visualBitmap.SetPixel(x, y, GetColorVal(pixelValues[x, y]));
                }
                colOffset++;
            }
        }





        //var data = visualBitmap.LockBits(new System.Drawing.Rectangle(0, 0, visualBitmap.Width, visualBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //Gdk.Pixmap pixmap = new Gdk.Pixmap(data.Scan0);



        currentColor++;
        if (currentColor > 1) { currentColor = 0; }

        //using (Cairo.Context cairoContext = CairoHelper.Create(base.GdkWindow))
        //{
        //    //cairoContext.
        //    for (int x = 0; x < pixelValues.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < pixelValues.GetLength(1); y++)
        //        {

        //        }
        //    }
        //}
        //g.MoveTo(0, 0);

        //g.LineTo(10, 10);
        //Cairo.Color color = new Cairo.Color(1.0, 0.5, 0.5);
        //g.SetSourceColor(color);
        //g.Rectangle(0, 0, 10, 10);
        //g.Fill();
        //g.Stroke();
        System.Console.WriteLine("Done executing");

        interpreter.EvaluateCode(interpreter.gotos[255], true);



    }

    Cairo.Color GetColorVal(int colorID)
    {
        //System.Drawing.Color newColor = System.Drawing.Color.FromArgb()
        switch (colorID)
        {
            case 0:
                return new Cairo.Color(0, 0, 0); // black
            case 1:
                return new Cairo.Color(255, 255, 255); // white
            case 2:
                return new Cairo.Color(255, 0, 0); // red
            case 3:
                return new Cairo.Color(0, 255, 0); // blue
            case 4:
                return new Cairo.Color(0, 0, 255); // green
            case 5:
                return new Cairo.Color(0, 255, 255); // cyan
            case 6:
                return new Cairo.Color(255, 0, 255); // magenta
            case 7:
                return new Cairo.Color(255, 255, 0); // yellow
            case 8:
                return new Cairo.Color(127, 127, 127); // grey
        }
        return new Cairo.Color(0, 0, 0); // give black if nothing else
    }
}
