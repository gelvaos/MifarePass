//----------------------------------------------------------------------------
//
// Copyright (c) 2011 Nodir Gulyamov <gelvaos at gmail.com>. 
//
// This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// By using this source code in any fashion, you are agreeing to be bound 
// by the terms of the Apache License, Version 2.0.
//
// You must not remove this notice, or any other, from this software.
//----------------------------------------------------------------------------

open System
open System.Windows.Forms
open System.Drawing

open MifarePassword

let createLabel (label_text, start_pos_x, start_pos_y, width, heigth) =
    let lbl = new Label ()
    lbl.AutoSize <- true
    lbl.Location <- new System.Drawing.Point (start_pos_x, start_pos_y)
    lbl.Size <- new System.Drawing.Size (width, heigth)
    lbl.Text <- label_text

    lbl

let createTextBox (textbox_txt, max_len, start_pos_x, start_pos_y, width, heigth, tabidx) =
    let tb = new TextBox ()
    tb.Text <- textbox_txt
    tb.MaxLength <- max_len
    tb.Location <- new System.Drawing.Point (start_pos_x, start_pos_y)
    tb.Size <- new System.Drawing.Size (width, heigth)
    tb.TabIndex <- tabidx

    tb


let createForm =
    let main_form = new Form()
    main_form.Width <- 310
    main_form.Height <- 300
    main_form.Visible <- true
    main_form.Text <- "MIFARE Password Calculator"

    let gb_input = new GroupBox ()
    let gb_output = new GroupBox ()
    gb_input.SuspendLayout ()
    gb_output.SuspendLayout ()


    // Menu
    let m_main = main_form.Menu <- new MainMenu ()
    let mi_file = main_form.Menu.MenuItems.Add ("&File")
    let mi_quit = new MenuItem ("&Quit")
    mi_file.MenuItems.Add (mi_quit) |> ignore
    let mi_help = main_form.Menu.MenuItems.Add("&Help")
    let mi_about = new MenuItem("&About")
    mi_help.MenuItems.Add (mi_about) |> ignore
    
    // texboxes and labels for Key A and Key B and Mifare password
    let lb_key_a = createLabel ("Key A:", 10, 23, 38, 13)
    let tb_key_a = createTextBox("Enter Key A here...", 12, 68, 19, 178, 20, 0)
    let lb_key_b = createLabel ("Key B:", 10, 49, 38, 13)
    let tb_key_b = createTextBox ("Enter Key B here...", 12, 68, 45, 178, 20, 1)
    
    gb_input.SuspendLayout ()
    gb_input.Controls.Add (lb_key_a)
    gb_input.Controls.Add (lb_key_b)
    gb_input.Controls.Add (tb_key_a)
    gb_input.Controls.Add (tb_key_b)
    gb_input.Location <- new System.Drawing.Point (12, 12)
    gb_input.Size <- new System.Drawing.Size (277, 80)
    gb_input.Text <- "Input: (Key A and Key B length should be 6 bytes)"
    

    let lb_dkey_a = createLabel ("Dkey A:", 13, 20, 45, 13)
    let tb_dkey_a = createTextBox ("", 12, 68, 20, 178, 20, 4)
    let lb_dkey_b = createLabel ("Dkey B:", 13, 49, 45, 13)
    let tb_dkey_b = createTextBox ("", 12, 68, 46, 178, 20, 5)
    let lb_mifare_pass = createLabel ("MIFARE\nPassword:", 10, 68, 56, 26)
    lb_mifare_pass.AutoSize <- true
    let tb_mifare_pass = createTextBox ("", 16, 68, 72, 178, 20, 6)

    gb_output.Controls.Add (lb_dkey_a)
    gb_output.Controls.Add (lb_dkey_b)
    gb_output.Controls.Add (tb_dkey_a)
    gb_output.Controls.Add (tb_dkey_b)
    gb_output.Controls.Add (lb_mifare_pass)
    gb_output.Controls.Add (tb_mifare_pass)
    gb_output.Location <- new System.Drawing.Point (12, 99)
    gb_output.Size <- new System.Drawing.Size (277, 109)
    gb_output.Text <- "Output:"



    let btn_calc = new Button ()
    btn_calc.Location <- new System.Drawing.Point(107, 214)
    btn_calc.Size <- new System.Drawing.Size(75, 23)
    btn_calc.TabIndex <- 3
    btn_calc.Text <- "Calculate"
    btn_calc.UseVisualStyleBackColor <- true


    main_form.Controls.Add (gb_input)
    main_form.Controls.Add (gb_output)
    main_form.Controls.Add (btn_calc)

    gb_input.ResumeLayout (false)
    gb_input.PerformLayout ()
    gb_output.ResumeLayout (false)
    gb_output.PerformLayout ()

    main_form.ResumeLayout (false)

    // callbacks
    mi_quit.Click.Add  (fun _ -> main_form.Close())
    mi_about.Click.Add (fun _ -> MessageBox.Show("MIFARE Password Calculator.\nAuthor: Nodir Gulyamov <gelvaos@gmail.com>"
                                                 , "About"
                                                 , MessageBoxButtons.OK
                                                 , MessageBoxIcon.Information) |> ignore)


    btn_calc.Click.Add (fun _ ->
                        if tb_key_a.Text.Length = 12 && tb_key_b.Text.Length = 12 then
                            let dkeya_str, dkeyb_str, pass_str = calcMifarePassword (tb_key_a.Text, tb_key_b.Text)
                            tb_dkey_a.Text <- dkeya_str
                            tb_dkey_b.Text <- dkeyb_str
                            tb_mifare_pass.Text <- pass_str
                        else
                            MessageBox.Show("They Key A or Key B lengths are incorrect."
                                           , "Error"
                                           , MessageBoxButtons.OK
                                           , MessageBoxIcon.Error) |> ignore)
                   


    main_form

#if COMPILED
[<STAThread>]
do Application.Run (createForm)
#endif

 
