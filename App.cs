
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCMateMainBranch;
using PCMateSubBranch;

namespace PCMate
{
    public partial class MainApp : Form
    {
        //temp counter --- will replace this to the json db instead
        Int16 counter = 0;

        //array containing the list of values for the flow Main Branch intent dropdown control
        string[,] arrayOfUserIntentValueNames = Globals.arrayOfUserIntentValueNames;
        //array containing the list of values and types for the flow Sub Branch Branch actions dropdown control
        string[,] arrayOfUserActionValueNames = Globals.arrayOfUserActionValueNames;

        string UtterenceSelectionTextboxPlaceholder = "Enter Utterance";
        string IntentSelectionPlaceholder = "Choose Intent";
        string ActionTypeSelectionPlaceholder = "Choose Action Type";
        string WebhookPlaceholder = "Enter URL";
        string SetChangeVolumePlaceholder = "{ Voice Controlled }";
        string SetVolumePlaceholder = "Enter Volume";
        string FileSelectParamsPlaceholder = "Enter Parameters";

        public MainApp()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            //Start the async server to accept connections
            Task.Run(() => { AsynchronousSocketListener asl = new AsynchronousSocketListener(); });

            AsynchronousSocketListener.updateGlobal += new AsynchronousSocketListener.newUtterance(updateMSG);


            string minimized = "false";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                minimized = appSettings["startminimized"] ?? "false";
            }
            catch (ConfigurationErrorsException)
            {
                minimized = "false";
            }

            if (minimized == "true")
            {
                //to minimize window
                this.WindowState = FormWindowState.Minimized;
                //to hide from taskbar
                this.ShowInTaskbar = false;
            }
        }


        private void updateMSG()
        {
            /*if (LastMessageTitle.InvokeRequired)
            {
                LastMessageTitle.Invoke(new MethodInvoker(delegate { Visible = true; }));
            }*/

            if (LastMessageTitle.InvokeRequired)
            {
                LastMessageTitle.Invoke(new MethodInvoker(delegate
                {
                    LastMessage.Text = AsynchronousSocketListener.lastmessage;
                    LastMessage.Visible = true;
                }));
            }


        }

        //<------Main Branch Settings----->

        //a method that is reponsible of creating main branch control
        void CreateMainBranch(string id, string intentValue = "", string utterenceValue = "", bool isDbGenerated = false, dynamic actionList = null, string intent = "")
        {
            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            counter++;

            //Main flow branch control settings
            MainBranch mainBranch = new MainBranch();
            mainBranch.Location = new Point(0, counter * 50);
            mainBranch.Dock = DockStyle.Top;
            mainBranch.Name = "mainBranch-" + id.ToString();
            mainBranch.Tag = "mainBranch";

            //Main branch intents Dropdown settings
            mainBranch.IntentSelectionChev.Click += new EventHandler(IntentSelectionDropdownMenu_Click);
            mainBranch.IntentSelectionValue.Click += new EventHandler(IntentSelectionDropdownMenu_Click);
            mainBranch.IntentSelectionValue.Click += new EventHandler(AllClicks);
            mainBranch.IntentSelectionValue.Name = "IntentSelectionValue-" + id.ToString();
            mainBranch.IntentSelectionValue.Text = intentValue.Length > 0 ? intentValue : IntentSelectionPlaceholder;
            //set the color of the dropdown menu based on content
            mainBranch.IntentSelectionValue.ForeColor = intentValue.Length > 0 ? Color.White : Color.FromArgb(145, 170, 157);
            //set the icon of the dropdown menu based on the content
            mainBranch.IntentSelectionIcon.BackgroundImage = intentValue.Length > 0 ? Image.FromFile("Images/Intent_Icon_Selected.png") : Image.FromFile("Images/Intent_Icon.png");
            mainBranch.IntentSelectionValue.Tag = intent;
            mainBranch.IntentSelectionIcon.Click += new EventHandler(IntentSelectionDropdownMenu_Click);


            //Settings for a Panel control that wraps the intent dropdown popup menu
            Panel IntentSelectionDropdownMenuWrapper = new Panel();
            IntentSelectionDropdownMenuWrapper.Location = new Point(50, mainBranch.Location.Y + 5);
            IntentSelectionDropdownMenuWrapper.Size = new Size(IntentSelectionDropdownMenuWrapper.Width, 200);
            IntentSelectionDropdownMenuWrapper.Visible = false;
            IntentSelectionDropdownMenuWrapper.Tag = "Dropdown-Popper";
            IntentSelectionDropdownMenuWrapper.Name = "IntentSelectionDropdownMenuWrapper-" + id.ToString();


            //Settings for the intent dropdown popup menu
            Panel IntentSelectionDropdownMenu = new Panel();
            IntentSelectionDropdownMenu.Size = new Size(171, 200);
            IntentSelectionDropdownMenu.Dock = DockStyle.Top;
            IntentSelectionDropdownMenu.MinimumSize = new Size(171, 100);
            IntentSelectionDropdownMenu.AutoScroll = true;
            IntentSelectionDropdownMenu.BackColor = Color.FromArgb(32, 68, 85);
            IntentSelectionDropdownMenu.Name = "IntentSelectionDropdownMenu-" + id.ToString();

            //generating a list of items in the intent dropdown pop up menu
            for (int i = 0; i < arrayOfUserIntentValueNames.Length / 2; i++)
            {

                Label IntentSelectionDropdownItemValue = new Label();
                IntentSelectionDropdownItemValue.Text = arrayOfUserIntentValueNames[i, 0];
                IntentSelectionDropdownItemValue.TextAlign = ContentAlignment.MiddleCenter;
                IntentSelectionDropdownItemValue.Dock = DockStyle.Top;
                IntentSelectionDropdownItemValue.ForeColor = Color.White;
                IntentSelectionDropdownItemValue.Padding = new Padding(0, 10, 0, 10);
                IntentSelectionDropdownItemValue.Size = new Size(IntentSelectionDropdownMenu.Width, 35);
                IntentSelectionDropdownItemValue.BackColor = Color.FromArgb(32, 68, 85);
                IntentSelectionDropdownItemValue.Location = new Point(0, 20 * i);
                IntentSelectionDropdownItemValue.Cursor = Cursors.Hand;
                IntentSelectionDropdownItemValue.Tag = arrayOfUserIntentValueNames[i, 1];
                IntentSelectionDropdownItemValue.Name = IntentSelectionDropdownMenu.Name + "-IntentSelectionDropdownItemValue-" + i.ToString();


                IntentSelectionDropdownItemValue.MouseEnter += new EventHandler(IntentSelectionDropdownItemValue_MouseEnter);
                IntentSelectionDropdownItemValue.MouseLeave += new EventHandler(IntentSelectionDropdownItemValue_MouseLeave);
                IntentSelectionDropdownItemValue.Click += new EventHandler(IntentSelectionDropdownItemValue_Click);

                IntentSelectionDropdownMenu.Controls.Add(IntentSelectionDropdownItemValue);
            }

            //Main branch utterence textbox settings
            mainBranch.UtterenceSelectionValue.TextAlign = ContentAlignment.MiddleLeft;
            mainBranch.UtterenceSelectionValue.Name = "UtterenceSelectionValue-" + id.ToString();
            mainBranch.UtterenceSelectionValue.Text = utterenceValue.Length > 0 ? utterenceValue : UtterenceSelectionTextboxPlaceholder;
            //set the color of the dropdown menu based on content
            mainBranch.UtterenceSelectionValue.ForeColor = utterenceValue.Length > 0 ? Color.White : Color.FromArgb(145, 170, 157);
            //set the icon of the dropdown menu based on the content
            mainBranch.UtterenceSelectionIcon.BackgroundImage = utterenceValue.Length > 0 ? Image.FromFile("Images/Utterence_Icon_Selected.png") : Image.FromFile("Images/Utterence_Icon.png");
            mainBranch.UtterenceSelectionValue.Click += new EventHandler(UtterenceSelectionValue_Click);

            //Main branch actions + button settings
            mainBranch.AddActionButtonValue.Name = "AddActionButtonValue-" + id.ToString();
            mainBranch.AddActionButtonValue.Click += new EventHandler(AddActionButton_Click);
            mainBranch.AddActionButtonValue.Click += new EventHandler(AllClicks);

            //run a function to set the save button to enabled
            ToggleSaveButtonState(SaveViewButton, false);

            //Main branch remove button settings
            mainBranch.RemoveMainBranchButton.Name = "RemoveMainBranchButton-" + id.ToString();
            mainBranch.RemoveMainBranchButton.Click += new EventHandler(RemoveMainBranchButton_Click);
            mainBranch.RemoveMainBranchButton.Click += new EventHandler(AllClicks);


            //Adding the newly defined main branch to a container in the form
            FlowsContainer.Controls.Add(mainBranch);
            //Add to intent dropdown menu popup wrapper the defined dropdown menu
            IntentSelectionDropdownMenuWrapper.Controls.Add(IntentSelectionDropdownMenu);
            //Add intent dropdown menu popup wrapper to a container in the form
            FlowsContainer.Controls.Add(IntentSelectionDropdownMenuWrapper);
            //Move the dropdown menu popup wrapper to the from of the controls layer hierarchy
            IntentSelectionDropdownMenuWrapper.BringToFront();

            //check if this is running as part of db branch generation or regular user interaction
            if (isDbGenerated)
            {
                foreach (var subflow in actionList)
                {
                    //run a function to create a new sub branch inside the main branch actions container
                    CreateSubBranch(mainBranch, subflow);
                }
            }
        }

        //a method that is responsible of handling the clicks on the main branch collapsible plus/minus button
        void AccordionCollapsibleButton_Click(object sender, EventArgs e)
        {
            PictureBox currentTarget = (sender as PictureBox);
            MainBranch mainBranch = currentTarget.Parent.Parent as MainBranch;

            //run function to toggle the view of the main branch collapsible based on the current visibility state
            ToggleMainBranchCollapsible(mainBranch, false);//set to false to allow toggling

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();
        }

        void ToggleMainBranchCollapsible(MainBranch mainBranch, bool openOnly)
        {
            //get number of children in the actions container
            int ActionsContainerCount = mainBranch.ActionsContainer.Controls.Count + 1;
            //set toggle button cursor to pointer hand
            mainBranch.AccordionCollapsibleButton.Cursor = Cursors.Hand;

            //check if function was called to only open or toggle the collapsible
            if (openOnly)
            {
                //update the image of the collapsible toggle button
                mainBranch.AccordionCollapsibleButton.BackgroundImage = Image.FromFile("Images/Close_Accordion_Button.png");
                //update the size of the main branch
                mainBranch.Size = new Size(mainBranch.ActionsContainer.Width, 55 * ActionsContainerCount);
            }
            else
            {
                //check if the size of the main branch is already in a closed state
                if (mainBranch.Size.Height > 55)//is "open"
                {
                    //update the image of the collapsible toggle button
                    mainBranch.AccordionCollapsibleButton.BackgroundImage = Image.FromFile("Images/Open_Accordion_Button.png");
                    //update the size of the main branch
                    mainBranch.Size = new Size(mainBranch.ActionsContainer.Width, 55);
                }
                else//is "closed"
                {
                    //update the image of the collapsible toggle button
                    mainBranch.AccordionCollapsibleButton.BackgroundImage = Image.FromFile("Images/Close_Accordion_Button.png");
                    //update the size of the main branch
                    mainBranch.Size = new Size(mainBranch.ActionsContainer.Width, 55 * ActionsContainerCount);
                }
            }

        }

        //a method responsible of controlling the behavior of the intent dropdown menu whenever the dropdown control is clicked
        void IntentSelectionDropdownMenu_Click(object sender, EventArgs e)
        {
            //set currently clicked target as general control
            Control currentTarget = (sender as Control);

            //get the main branch control
            Control currentTargetMainBranch = currentTarget.Parent.Parent.Parent.Parent.Parent;
            //get the mainbranch name
            string currentTargetMainBranchName = currentTargetMainBranch.Name;
            //split name into arrays based on the dash sign
            string[] currentTargetMainBranchNameList = currentTargetMainBranchName.Split(new Char[] { '-' });
            //get the id of the relevant branch
            string currentTargetId = currentTargetMainBranchNameList[1];
            //fin the relevant popup wrapper based on the id
            Control[] IntentSelectionDropdownMenuWrapper = FlowsContainer.Controls.Find("IntentSelectionDropdownMenuWrapper-" + currentTargetId, true);

            //run method to toggle the menu visibility
            HandleIntentDropDownToggle(IntentSelectionDropdownMenuWrapper[0], currentTargetMainBranchName);
        }

        //a function responsible of handling the toggling behaviour of the intent dropdown menu
        void HandleIntentDropDownToggle(Control IntentSelectionDropdownMenuWrapper, string currentTargetMainBranchName)
        {
            //get main branch based on branch name
            Control[] mainBranches = FlowsContainer.Controls.Find(currentTargetMainBranchName, true);

            //check if the menu wrapper is already visible
            if (IntentSelectionDropdownMenuWrapper.Visible)
            {
                //switch visibility to false
                IntentSelectionDropdownMenuWrapper.Visible = false;
            }
            else
            {
                //hides all the controls tagged with "Dropdown-Popper"
                HideAllDropdownPoppers();
                //update the position of the menu wrapper on the screen
                IntentSelectionDropdownMenuWrapper.Location = new Point(50, mainBranches[0].Location.Y + 55);
                //switch visibility to true
                IntentSelectionDropdownMenuWrapper.Visible = true;
            }

        }

        //a method responsible of controlling the intent dropdown items whenever mouse enter event is triggered
        void IntentSelectionDropdownItemValue_MouseEnter(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            //update background color of dropdown item button
            currentTarget.BackColor = Color.FromArgb(62, 96, 111);
            //mainBranch.Size = new Size(mainBranch.Width, 200);

        }

        //a method responsible of controlling the intent dropdown items whenever mouse leave event is triggered
        void IntentSelectionDropdownItemValue_MouseLeave(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            //update background color of dropdown item button
            currentTarget.BackColor = Color.FromArgb(32, 68, 85);
        }

        //a method responsible of handling the selection of an intent fromt he dropdown menu
        void IntentSelectionDropdownItemValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            //get the name of the current target
            string currentTargetDropdownItemName = currentTarget.Name;
            //split the name into an array based on the dash character
            string[] currentTargetDropdownItemNameList = currentTargetDropdownItemName.Split(new Char[] { '-' });
            //set name of current main branch based on the obtained id
            string currentTargetMainBranchName = "mainBranch-" + currentTargetDropdownItemNameList[1];
            //find the main branch control relevant to the clicked element
            Control[] IntentSelectionValues = FlowsContainer.Controls.Find(currentTargetMainBranchName, true);

            //set the found control as MainBranch type of control
            MainBranch IntentSelectionValue = IntentSelectionValues[0] as MainBranch;
            //update the color of the dropdown menu
            IntentSelectionValue.IntentSelectionValue.ForeColor = Color.White;
            //update the icon of the dropdown menu
            IntentSelectionValue.IntentSelectionIcon.BackgroundImage = Image.FromFile("Images/Intent_Icon_Selected.png");
            //update the value of the dropdown menu
            IntentSelectionValue.IntentSelectionValue.Text = currentTarget.Text;
            //add item tag to intent selection value as a tag
            IntentSelectionValue.IntentSelectionValue.Tag = currentTarget.Tag;

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();

        }


        //a method responsible of handling a click on the utterence textbox label
        void UtterenceSelectionValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            //get the main branch control relevant to the current control
            MainBranch mainBranch = currentTarget.Parent.Parent.Parent.Parent as MainBranch;


            //generate a new textbox within the label
            MaskedTextBox ControlTextbox = GenerateTextBox(currentTarget, 12, Color.White, Color.FromArgb(32, 68, 85), new Point(4, 11));
            //assign event listeners specific to the related control
            ControlTextbox.LostFocus += new EventHandler(UtterenceSelectionTextbox_LostFocus);
            ControlTextbox.KeyDown += new KeyEventHandler(UtterenceSelectionTextbox_EnterKeyDown);
            //set the focus on the newly created textbox
            ControlTextbox.Focus();
            ControlTextbox.SelectAll();

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //empty the utterence textbox label
            //currentTarget.Text = "";
        }

        //a method responsible of handling the loss of focus on an utterence textbox
        void UtterenceSelectionTextbox_LostFocus(object sender, EventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);
            //get main branch of the relevant control
            MainBranch mainBranch = currentTarget.Parent.Parent.Parent.Parent.Parent.Parent as MainBranch;
            //run a function to update the value of the utterence textbox label value
            UpdateUtterenceSelectionValue(currentTarget, mainBranch);
        }

        //a method responsible of handling keypress on an utterence textbox
        void UtterenceSelectionTextbox_EnterKeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);
            //get main branch of the relevant control
            MainBranch mainBranch = currentTarget.Parent.Parent.Parent.Parent.Parent.Parent as MainBranch;

            //check if keyboard key is equal to any of the keys below
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Escape)
            {
                //run a function to update the value of the utterence textbox label value
                UpdateUtterenceSelectionValue(currentTarget, mainBranch);
                e.Handled = true;
                e.SuppressKeyPress = true;
                //Scroll back to tab temporary bugfix
                SendKeys.Send("+{TAB}");
                FlowsContainer.VerticalScroll.Value = 0;


            }



        }

        //a method responsible of updating the value of the utterence textbox label value
        void UpdateUtterenceSelectionValue(MaskedTextBox currentTarget, MainBranch mainBranch)
        {
            //check if utterence textbox has any value
            if (currentTarget.Text.Length > 0 && currentTarget.Text != UtterenceSelectionTextboxPlaceholder)//has value
            {
                //set the text of the utterence textbox label to the one entered by the user in the textbox
                currentTarget.Parent.Text = currentTarget.Text;
                //update the color of the utterence textbox label
                currentTarget.Parent.ForeColor = Color.White;
                //remove the textbox
                currentTarget.Parent.Controls.Remove(currentTarget);
                //update the icon of the uttterence selction
                mainBranch.UtterenceSelectionIcon.BackgroundImage = Image.FromFile("Images/Utterence_Icon_Selected.png");
            }
            else//no value
            {
                //set the text of the utterence textbox label to the default placeholder
                currentTarget.Parent.Text = UtterenceSelectionTextboxPlaceholder;
                //update the color of the utterence textbox label
                currentTarget.Parent.ForeColor = Color.FromArgb(145, 170, 157);
                //remove the textbox
                currentTarget.Parent.Controls.Remove(currentTarget);
                //update the icon of the uttterence selction
                mainBranch.UtterenceSelectionIcon.BackgroundImage = Image.FromFile("Images/Utterence_Icon.png");
            }

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();

        }


        //hides all the controls tagged with "Dropdown-Popper"
        void HideAllDropdownPoppers()
        {
            //run through every 1st level child of the flows container
            foreach (Control c in FlowsContainer.Controls)
            {
                string tag = c.Tag as string;
                //check if the relevnat control tag is a dropdown popper
                if (tag == "Dropdown-Popper")
                {
                    //hide the control
                    c.Visible = false;
                }
            }
        }

        //a method responsible of handling the clicks on the actions+ button
        void AddActionButton_Click(object sender, EventArgs e)
        {

            Label currentTarget = (sender as Label);
            //get main branch control
            MainBranch mainBranch = currentTarget.Parent.Parent.Parent.Parent.Parent.Parent as MainBranch;

            //run a function to create a new sub branch inside the main branch actions container
            CreateSubBranch(mainBranch);

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation(disable: true);
        }

        //a method responsible of handling the clicks on the save button
        void SaveButton_Click(object sender, EventArgs e)
        {

            //string newjson = @"{
            //                        'id': '1',
            //                        'userintentvalue': 'dick',
            //                        'intent': 'turn on',
            //                        'utterance': 'chrome',
            //                        'synonyms': 'test,test,test',
            //                        'actionlist': [
            //                            {
            //                                'actiontype': 'launch',
            //                                'param1': 'C:\\Program Files (x86)\\Google\\Chrome\\Application\\Chrome.exe',
            //                                'param2':'youtube.com',
            //                                'param3': '',
            //                                'param4': '',
            //                                'useractionvalue': 'Open Program'
            //                            }
            //                        ]
            //                    }";
            //Globals.jsondb = JsonConvert.DeserializeObject(newjson);

            string newJson = "[";
            int index = 0;
            //run through every 1st level child of the flows container
            foreach (Control c in FlowsContainer.Controls)
            {
                index++;
                string tag = c.Tag as string;
                MainBranch mainBranch = c as MainBranch;
                //check if the relevnat control tag is a main branch
                if (tag == "mainBranch")
                {
                    string actionlist = "";
                    newJson += "{";
                    //get the name list of the main branch and split it to array based on dash character
                    string[] mainBranchNameNameList = mainBranch.Name.Split(new Char[] { '-' });
                    //get the main branch id
                    string mainBranchId = mainBranchNameNameList[1];

                    //add value from the relevant main branch into the new json string
                    newJson += "\"id\":\"" + mainBranchId + "\",";
                    newJson += "\"userintentvalue\":\"" + mainBranch.IntentSelectionValue.Text + "\",";
                    newJson += "\"intent\": \"" + mainBranch.IntentSelectionValue.Tag + "\",";
                    newJson += "\"utterance\": \"" + mainBranch.UtterenceSelectionValue.Text + "\",";
                    newJson += "\"synonyms\": \"\",";

                    //get the number of sub branches in each main branch
                    int subBranchesCount = mainBranch.ActionsContainer.Controls.Count;

                    //run through the sub branches of the current main branch
                    for (int s = subBranchesCount - 1; s >= 0; s--)
                    {
                        SubBranch subBranch = mainBranch.ActionsContainer.Controls[s] as SubBranch;
                        //get the action type from the tag of the action selected
                        string ActionValueTypeTag = subBranch.ChooseActionTypeValue.Tag.ToString();
                        //split the tag name into an array based on dash seperation
                        string[] ActionValueTypeTagsList = ActionValueTypeTag.Split(new Char[] { '-' });


                        //create json string from actionslist
                        actionlist += "{";
                        actionlist += "\"actiontype\":\"" + ActionValueTypeTagsList[0] + "\",";

                        //show the selected control in the relevant sub branch
                        switch (ActionValueTypeTagsList[0])
                        {
                            case "launch":
                                string FileSelectControlParamValue = subBranch.FileSelectControlParamValue.Text == FileSelectParamsPlaceholder ? "" : subBranch.FileSelectControlParamValue.Text;
                                actionlist += "\"param1\":\"" + subBranch.FileSelectControlFileName.Tag.ToString().Replace(@"\", @"\\") + "\",";
                                actionlist += "\"param2\":\"" + FileSelectControlParamValue + "\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                            case "setvolume":
                                actionlist += "\"param1\":\"" + subBranch.SetVolumeControlValue.Text + "\",";
                                actionlist += "\"param2\":\"\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                            case "changevolume":
                                actionlist += "\"param1\":\"" + ActionValueTypeTagsList[1] + "\",";
                                actionlist += "\"param2\":\"" + subBranch.SetVolumeControlValue.Text + "\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                            case "changeaudiodevice":
                                actionlist += "\"param1\":\"" + subBranch.DropDownControlValue.Text + "\",";
                                actionlist += "\"param2\":\"\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                            case "screencontrol":
                                actionlist += "\"param1\":\"" + ActionValueTypeTagsList[1] + "\",";
                                actionlist += "\"param2\":\"\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                            case "timetask":
                                if (ActionValueTypeTagsList[1] == "delay")
                                {
                                    actionlist += "\"param1\":\"" + ActionValueTypeTagsList[1] + "\",";
                                    actionlist += "\"param2\":\"" + subBranch.DurationHhValue.Text + ":" + subBranch.DurationMmValue.Text + ":" + subBranch.DurationSsValue.Text + "\",";
                                    actionlist += "\"param3\":\"\",";
                                    actionlist += "\"param4\":\"\",";
                                }
                                else if (ActionValueTypeTagsList[1] == "scheduled")
                                {

                                }
                                break;
                            case "computertask":
                                actionlist += "\"param1\":\"" + ActionValueTypeTagsList[1] + "\",";
                                actionlist += "\"param2\":\"\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                            case "webhook":
                                actionlist += "\"param1\":\"" + subBranch.WebhookControlValue.Text.Replace(@"\", @"\\") + "\",";
                                actionlist += "\"param2\":\"\",";
                                actionlist += "\"param3\":\"\",";
                                actionlist += "\"param4\":\"\",";
                                break;
                        }
                        actionlist += "\"useractionvalue\":\"" + subBranch.ChooseActionTypeValue.Text + "\"";
                        actionlist += s == 0 ? "}" : "},";
                    }

                    newJson += "\"actionlist\": [" + actionlist + "]";
                    newJson += FlowsContainer.Controls.Count == index ? "}" : "},";
                }
            }
            newJson += "]";

            //Save new JSON to file and to global variable
            File.WriteAllText(@"db.json", newJson);
            Globals.jsondb = JsonConvert.DeserializeObject(newJson);

            //run a function to set the save button to disabled
            ToggleSaveButtonState(SaveViewButton, false);
        }

        //a method responsible of checking if the branch save button should be enabled based on the conditions
        void BranchSaveValidation(bool disable = false)
        {
            //check if disable boolean is true and if it is disable save button without checking
            if (disable)
            {
                //run a function to set the save button to disabled
                ToggleSaveButtonState(SaveViewButton, false);
            }
            else
            {
                //get count of main branches
                int mainBranchCount = FlowsContainer.Controls.Count;

                //set bool to change based on number of sub branches each main branch has
                bool hasNoSubranches = false;

                //set bool to change if there are no more main branches
                bool hasNoMainBranches = true;

                //set bool to change based on all sub branches validity
                bool areSubBranchesInvalid = false;
                //set bool to change based on selected sub branch control validity
                bool areSubBranchControlsInvalid = false;

                //set bool to change based on selected main branchvalidity
                bool areMainBranchesInvalid = false;

                //run through all the existing main branches and check if all conditions are met to enable/disable save button
                for (int m = 0; m < mainBranchCount; m++)
                {
                    //get tag of the flows container 
                    string tag = FlowsContainer.Controls[m].Tag as string;

                    //check if the relevnat control tag is a main branch
                    if (tag == "mainBranch")
                    {
                        //set the main branch bool to true 
                        hasNoMainBranches = false;

                        MainBranch mainBranch = FlowsContainer.Controls[m] as MainBranch;

                        //get current branch intent selection value
                        string intentValue = mainBranch.IntentSelectionValue.Text as string;

                        //get current branch utterance value
                        string utteranceValue = mainBranch.UtterenceSelectionValue.Text as string;

                        //check if main branch intent and utterance are in valid condition
                        if ((intentValue.Length <= 0 && utteranceValue.Length <= 0) || intentValue == IntentSelectionPlaceholder || utteranceValue == UtterenceSelectionTextboxPlaceholder)
                        {
                            areMainBranchesInvalid = true;
                        }

                        //get count of main branch sub branch
                        int subBranchCount = mainBranch.ActionsContainer.Controls.Count;

                        //run through all the existing sub branches and check if all conditions are met to enable/disable save button
                        for (int s = 0; s < subBranchCount; s++)
                        {
                            SubBranch subBranch = mainBranch.ActionsContainer.Controls[s] as SubBranch;

                            //get the action type from the tag of the action selected
                            string ActionValueTypeTag = subBranch.ChooseActionTypeValue.Tag != null ? subBranch.ChooseActionTypeValue.Tag.ToString() : "";
                            //split the tag name into an array based on dash seperation
                            string[] ActionValueTypeTagsList = ActionValueTypeTag.Split(new Char[] { '-' });

                            //show the selected control in the relevant sub branch
                            switch (ActionValueTypeTagsList[0])
                            {
                                case "launch":
                                    //get current branch sub branch file select param value
                                    string fileSelectControlFileName = subBranch.FileSelectControlFileName.Text as string;

                                    if (fileSelectControlFileName.Length <= 0)
                                    {
                                        areSubBranchControlsInvalid = true;
                                    }
                                    break;
                                case "setvolume":
                                case "changevolume":
                                    //check if checkbox is active or not
                                    if (subBranch.SetVolumeControlCheckbox.Checked)
                                    {
                                        if (SetVolumePlaceholder == subBranch.SetVolumeControlValue.Text)
                                        {
                                            areSubBranchControlsInvalid = true;
                                        }
                                    }
                                    break;
                                case "screencontrol":
                                    break;
                                case "timetask":
                                    if (ActionValueTypeTagsList[1] == "delay")
                                    {
                                        if (subBranch.DurationHhValue.Text == "hh" || subBranch.DurationMmValue.Text == "mm" || subBranch.DurationSsValue.Text == "ss")
                                        {
                                            areSubBranchControlsInvalid = true;
                                        }
                                    }
                                    else if (ActionValueTypeTagsList[1] == "scheduled")
                                    {

                                    }
                                    break;
                                case "computertask":
                                    break;
                                case "webhook":
                                    if (subBranch.WebhookControlValue.Text == WebhookPlaceholder)
                                    {
                                        areSubBranchControlsInvalid = true;
                                    }
                                    break;
                                case "changeaudiodevice":
                                    if (subBranch.DropDownControlValue.Text == "No audio devices found" || subBranch.DropDownControlValue.Text == "Select Audio Device")
                                    {
                                        areSubBranchControlsInvalid = true;
                                    }
                                    break;
                            }

                            //get current branch sub branch action type value
                            string actionTypeValue = subBranch.ChooseActionTypeValue.Text as string;

                            //check if sub branch action type value and relevant control values are filled
                            if (actionTypeValue == ActionTypeSelectionPlaceholder)
                            {
                                areSubBranchesInvalid = true;
                            }
                        }

                        //check if main branch has any sub branches
                        if (mainBranch.ActionsContainer.Controls.Count == 0)
                        {
                            hasNoSubranches = true;
                        }

                    }

                }
                //check if there are no main branches to run through and if there aren't activate save button 
                if (hasNoMainBranches)
                {
                    //run a function to set the save button to disabled
                    ToggleSaveButtonState(SaveViewButton, true);
                }
                else
                {
                    //check if user met all the conditions for saving the view
                    if (areMainBranchesInvalid || hasNoSubranches || areSubBranchesInvalid || areSubBranchControlsInvalid)
                    {
                        //run a function to set the save button to disabled
                        ToggleSaveButtonState(SaveViewButton, false);
                    }
                    else
                    {
                        //run a function to set the save button to enabled
                        ToggleSaveButtonState(SaveViewButton, true);
                    }
                }
            }


        }

        //a methond that is responsible of setting the save button to enabled/disabled based ont he parameters states
        void ToggleSaveButtonState(Button SaveViewButton, bool Enabled)
        {
            //if enabled
            if (Enabled)
            {
                //update the button backcolor
                SaveViewButton.BackColor = Color.FromArgb(62, 96, 111);
                //update the button text color
                SaveViewButton.ForeColor = Color.White;
                //update the cursor of the button to hand pointer
                SaveViewButton.Cursor = Cursors.Hand;
                //enable button
                SaveViewButton.Enabled = true;
            }
            else
            {
                //update the button backcolor
                SaveViewButton.BackColor = Color.Silver;
                //update the button text color
                SaveViewButton.ForeColor = Color.Transparent;
                //update the cursor of the button to default
                SaveViewButton.Cursor = Cursors.Default;
                //disable button
                SaveViewButton.Enabled = false;
            }

        }

        //a method that is responsible of handling the click on the remove main branch button
        void RemoveMainBranchButton_Click(object sender, EventArgs e)
        {
            PictureBox currentTarget = (sender as PictureBox);
            //obtain the main branch control of the currently clicked control
            MainBranch mainBranch = currentTarget.Parent.Parent.Parent.Parent as MainBranch;
            //get the name list of the main branch and split it to array based on dash character
            string[] mainBranchNameNameList = mainBranch.Name.Split(new Char[] { '-' });
            //get the main branch id
            string mainBranchId = mainBranchNameNameList[1];
            //set the current intent dropdown menu wrapper name
            string IntentSelectionDropdownMenuWrapperName = "IntentSelectionDropdownMenuWrapper-" + mainBranchId;
            //find the wrapper control based on the name
            Control[] IntentSelectionDropdownMenuWrapperValues = FlowsContainer.Controls.Find(IntentSelectionDropdownMenuWrapperName, true);
            //remove the wrapper control along with all its children
            IntentSelectionDropdownMenuWrapperValues[0].Parent.Controls.Remove(IntentSelectionDropdownMenuWrapperValues[0]);
            //remove the main branch control including all its sub branches
            FlowsContainer.Controls.Remove(mainBranch);
            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }


        //<------Sub Branch Settings----->

        //a method responsible of creating a new sub branch inside the main branch actions container
        void CreateSubBranch(MainBranch mainBranch, dynamic subflow = null)
        {
            //amount of sub branches for the specific main branch
            int ActionsCount = mainBranch.ActionsContainer.Controls.Count;

            //check if there are not branches yet
            if (ActionsCount == 0)
            {
                //update the accordion toggle button image
                mainBranch.AccordionCollapsibleButton.BackgroundImage = Image.FromFile("Images/Open_Accordion_Button.png");
                //update the accordion toggle button click event
                mainBranch.AccordionCollapsibleButton.Click += new EventHandler(AccordionCollapsibleButton_Click);
                mainBranch.AccordionCollapsibleButton.Click += new EventHandler(AllClicks);
                //update the accordion toggle button cursor to pointer hand
                mainBranch.AccordionCollapsibleButton.Cursor = Cursors.Hand;
            }

            ActionsCount++;

            //Sub branch settings
            SubBranch subBranch = new SubBranch();
            subBranch.Dock = DockStyle.Top;
            subBranch.Name = mainBranch.Name + "-subBranch-" + ActionsCount.ToString();

            //Sub branch action type dropdown button settings
            subBranch.ChooseActionTypeChev.Click += new EventHandler(ActionSelectionDropdownMenu_Click);
            subBranch.ChooseActionTypeChev.Name = mainBranch.Name + "-ChooseActionTypeChev-" + ActionsCount.ToString();
            subBranch.ChooseActionTypeValue.Click += new EventHandler(ActionSelectionDropdownMenu_Click);
            subBranch.ChooseActionTypeValue.Click += new EventHandler(AllClicks);
            subBranch.ChooseActionTypeValue.Text = subflow != null ? subflow.useractionvalue : "Choose Action Type";
            subBranch.ChooseActionTypeValue.Name = mainBranch.Name + "-ChooseActionTypeValue-" + ActionsCount.ToString();

            //Sub branch action type dropdown menu wrapper settings
            Panel ActionSelectionDropdownMenuWrapper = new Panel();
            ActionSelectionDropdownMenuWrapper.Location = new Point(50, subBranch.Location.Y + 5);
            ActionSelectionDropdownMenuWrapper.Visible = false;
            ActionSelectionDropdownMenuWrapper.Tag = "Dropdown-Popper";
            ActionSelectionDropdownMenuWrapper.Name = mainBranch.Name + "-ActionSelectionDropdownMenuWrapper-" + ActionsCount.ToString();
            ActionSelectionDropdownMenuWrapper.Size = new Size(ActionSelectionDropdownMenuWrapper.Width, 200);


            //Sub branch action type dropdown menu
            Panel ActionSelectionDropdownMenu = new Panel();
            ActionSelectionDropdownMenu.Size = new Size(171, 200);
            ActionSelectionDropdownMenu.Dock = DockStyle.Top;
            ActionSelectionDropdownMenu.MinimumSize = new Size(171, 50);
            ActionSelectionDropdownMenu.AutoScroll = true;
            ActionSelectionDropdownMenu.BackColor = Color.FromArgb(232, 235, 236);
            ActionSelectionDropdownMenu.Name = "ActionSelectionDropdownMenu-" + ActionsCount.ToString();

            //check if function is running as part of db generation
            if (subflow != null)
            {
                //set the default values and view of the controls inside the sub branch if any exist
                SetSelectedSubBranchControl(subBranch, Convert.ToString(subflow.actiontype), subflow);
            }

            //Sub branch custom controls settings
            //webhook
            subBranch.WebhookControlValue.Name = mainBranch.Name + "-WebhookControlValue-" + ActionsCount.ToString();
            subBranch.WebhookControlValue.ForeColor = Color.FromArgb(62, 96, 111);
            subBranch.WebhookControlValue.Click += new EventHandler(WebhookControlValue_Click);
            //set volume
            subBranch.SetVolumeControlCheckbox.Name = mainBranch.Name + "-SetVolumeControlCheckbox-" + ActionsCount.ToString();
            subBranch.SetVolumeControlCheckbox.CheckedChanged += new EventHandler(SetVolumeControlCheckbox_CheckedChanged);
            subBranch.SetVolumeControlCheckbox.Click += new EventHandler(AllClicks);
            subBranch.SetVolumeControlValue.Name = mainBranch.Name + "-SetVolumeControlValue-" + ActionsCount.ToString();
            //set duration
            subBranch.DurationHhValue.Name = mainBranch.Name + "-DurationHhValue-" + ActionsCount.ToString();
            subBranch.DurationMmValue.Name = mainBranch.Name + "-DurationMmValue-" + ActionsCount.ToString();
            subBranch.DurationSsValue.Name = mainBranch.Name + "-DurationSsValue-" + ActionsCount.ToString();
            subBranch.DurationHhValue.Click += new EventHandler(DurationControlValue_Click);
            subBranch.DurationMmValue.Click += new EventHandler(DurationControlValue_Click);
            subBranch.DurationSsValue.Click += new EventHandler(DurationControlValue_Click);

            //open program
            subBranch.FileSelectControlChooseFileButton.Name = mainBranch.Name + "-FileSelectControlChooseFileButton-" + ActionsCount.ToString();
            subBranch.FileSelectControlChooseFileButton.Click += new EventHandler(FileSelectControlChooseFileButton_Click);
            subBranch.FileSelectControlChooseFileButton.Click += new EventHandler(AllClicks);
            subBranch.FileSelectControlParamValue.Click += new EventHandler(FileSelectControlParamValue_Click);

            //run through all action value names and create dropdown item controls inside dropdown menu
            for (int i = 0; i < arrayOfUserActionValueNames.Length / 3; i++)
            {
                //Actions dropdown item settings
                Label ActionSelectionDropdownMenuItemValue = new Label();
                ActionSelectionDropdownMenuItemValue.Text = arrayOfUserActionValueNames[i, 0];
                ActionSelectionDropdownMenuItemValue.TextAlign = ContentAlignment.MiddleCenter;
                ActionSelectionDropdownMenuItemValue.Dock = DockStyle.Top;
                ActionSelectionDropdownMenuItemValue.ForeColor = Color.FromArgb(32, 68, 85);
                ActionSelectionDropdownMenuItemValue.Padding = new Padding(0, 10, 0, 10);
                ActionSelectionDropdownMenuItemValue.Size = new Size(ActionSelectionDropdownMenu.Width, 35);
                ActionSelectionDropdownMenuItemValue.BackColor = Color.FromArgb(232, 235, 236);
                ActionSelectionDropdownMenuItemValue.Location = new Point(0, 20 * i);
                ActionSelectionDropdownMenuItemValue.Cursor = Cursors.Hand;
                ActionSelectionDropdownMenuItemValue.Name = mainBranch.Name + "-" + ActionSelectionDropdownMenu.Name + "-ActionSelectionDropdownItemValue-" + i.ToString();
                ActionSelectionDropdownMenuItemValue.Tag = arrayOfUserActionValueNames[i, 1] + "-" + arrayOfUserActionValueNames[i, 2];

                ActionSelectionDropdownMenuItemValue.MouseEnter += new EventHandler(ActionSelectionDropdownMenuItemValue_MouseEnter);
                ActionSelectionDropdownMenuItemValue.MouseLeave += new EventHandler(ActionSelectionDropdownMenuItemValue_MouseLeave);
                ActionSelectionDropdownMenuItemValue.Click += new EventHandler(ActionSelectionDropdownMenuItemValue_Click);

                ActionSelectionDropdownMenu.Controls.Add(ActionSelectionDropdownMenuItemValue);
            }

            //audio dropdown
            subBranch.DropDownControlValue.Click += new EventHandler(AudioDeviceDropdownMenu_Click);
            subBranch.DropDownControlValue.Name = mainBranch.Name + "-DropDownControlValue-" + ActionsCount.ToString();


            //ACTION DROP DOWN (AUDIO DEVICE SELECT)
            //Settings for a Panel control that wraps the intent dropdown popup menu
            Panel AudioDeviceDropdownMenuWrapper = new Panel();
            AudioDeviceDropdownMenuWrapper.Size = new Size(AudioDeviceDropdownMenuWrapper.Width, 200);
            AudioDeviceDropdownMenuWrapper.Visible = false;
            AudioDeviceDropdownMenuWrapper.Tag = "Dropdown-Popper";
            AudioDeviceDropdownMenuWrapper.Name = mainBranch.Name + "-AudioDeviceDropdownMenuWrapper-" + ActionsCount.ToString();


            //Settings for the intent dropdown popup menu
            Panel AudioDeviceDropdownMenu = new Panel();
            AudioDeviceDropdownMenu.Size = new Size(171, 200);
            AudioDeviceDropdownMenu.Dock = DockStyle.Top;
            AudioDeviceDropdownMenu.MinimumSize = new Size(171, 100);
            AudioDeviceDropdownMenu.AutoScroll = true;
            AudioDeviceDropdownMenu.BackColor = Color.FromArgb(232, 235, 236);
            AudioDeviceDropdownMenu.Name = "AudioDeviceDropdownMenu-" + ActionsCount.ToString();

            //generating a list of items in the intent dropdown pop up menu
            for (int i = 0; i < Globals.audiodevices.Count; i++)
            {

                Label AudioDeviceDropdownItemValue = new Label();
                AudioDeviceDropdownItemValue.Text = Globals.audiodevices[i];
                AudioDeviceDropdownItemValue.TextAlign = ContentAlignment.MiddleCenter;
                AudioDeviceDropdownItemValue.Dock = DockStyle.Top;
                AudioDeviceDropdownItemValue.ForeColor = Color.FromArgb(32, 68, 85);
                AudioDeviceDropdownItemValue.Padding = new Padding(0, 10, 0, 10);
                AudioDeviceDropdownItemValue.Size = new Size(AudioDeviceDropdownMenu.Width, 35);
                AudioDeviceDropdownItemValue.Location = new Point(0, 20 * i);
                AudioDeviceDropdownItemValue.Cursor = Cursors.Hand;
                AudioDeviceDropdownItemValue.Tag = arrayOfUserIntentValueNames[i, 1];
                AudioDeviceDropdownItemValue.Name = mainBranch.Name + "-" + AudioDeviceDropdownMenu.Name + "-AudioDeviceDropdownItemValue-" + i.ToString();

                AudioDeviceDropdownItemValue.Click += new EventHandler(AudioDeviceItemValue_Click);

                AudioDeviceDropdownMenu.Controls.Add(AudioDeviceDropdownItemValue);
            }


            //Remove sub branch button settings
            subBranch.RemoveSubBranchButton.Name = mainBranch.Name + "-RemoveSubBranchButton-" + ActionsCount.ToString();
            subBranch.RemoveSubBranchButton.Click += new EventHandler(RemoveSubBranchButton_Click);
            subBranch.RemoveSubBranchButton.Click += new EventHandler(AllClicks);

            //add sub branch to actions container of the relevant main branch
            mainBranch.ActionsContainer.Controls.Add(subBranch);
            //set the sub branch index to the end of the list
            mainBranch.ActionsContainer.Controls.SetChildIndex(subBranch, 0);
            //add actions dropdown menu to actions dropdown wrapper
            ActionSelectionDropdownMenuWrapper.Controls.Add(ActionSelectionDropdownMenu);
            AudioDeviceDropdownMenuWrapper.Controls.Add(AudioDeviceDropdownMenu);
            //add actions dropdown wrapper to flows container
            FlowsContainer.Controls.Add(ActionSelectionDropdownMenuWrapper);
            FlowsContainer.Controls.Add(AudioDeviceDropdownMenuWrapper);
            //move actions dropdown wrapper to the front of the control layers hierarchy
            ActionSelectionDropdownMenuWrapper.BringToFront();
            AudioDeviceDropdownMenuWrapper.BringToFront();


            //check if function is running as part of db generation
            if (subflow == null)
            {
                //run the toggle collapsible function to ensure the collapsible opens up if needed
                ToggleMainBranchCollapsible(mainBranch, true);
            }

        }

        //a methond that is responsible of click on the actions selection dropdown control
        void ActionSelectionDropdownMenu_Click(object sender, EventArgs e)
        {
            Control currentTarget = (sender as Control);
            //get sub branch control
            Control currentTargetSubBranch = currentTarget.Parent.Parent.Parent.Parent;
            //get the current y-axis position of the sub branch
            int currentTargetSubBranchPositionY = currentTargetSubBranch.FindForm().PointToClient(currentTargetSubBranch.Parent.PointToScreen(currentTargetSubBranch.Location)).Y - 27;
            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];

            //get the actions dropdown wrapper control
            Control[] ActionSelectionDropdownMenuWrapper = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-ActionSelectionDropdownMenuWrapper-" + subBranchId, true);

            //check if the dropdown wrapper is visible already
            if (ActionSelectionDropdownMenuWrapper[0].Visible)
            {
                //hide the dropdown menu wrapper
                ActionSelectionDropdownMenuWrapper[0].Visible = false;
            }
            else
            {
                //hides all the controls tagged with "Dropdown-Popper"
                HideAllDropdownPoppers();
                //update the position of the dropdown menu wrapper
                ActionSelectionDropdownMenuWrapper[0].Location = new Point(50, currentTargetSubBranchPositionY);
                //show the dropdown menu wrapper
                ActionSelectionDropdownMenuWrapper[0].Visible = true;
            }

        }

        //a method responsible of mouse enter event on the sub branch action dropdown item
        void ActionSelectionDropdownMenuItemValue_MouseEnter(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            //update background color of dropdown item
            currentTarget.BackColor = Color.FromArgb(209, 214, 217);

        }

        //a method responsible of mouse leave event on the sub branch action dropdown item
        void ActionSelectionDropdownMenuItemValue_MouseLeave(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            //update background color of dropdown item
            currentTarget.BackColor = Color.FromArgb(232, 235, 236);
        }

        //a method responsible of click event on the sub branch action dropdown item
        void ActionSelectionDropdownMenuItemValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);

            //get name of the current button clicked and split to array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //get main branch id
            string mainBranchId = currentTargetNameList[1];
            //get sub branch id
            string subBranchId = currentTargetNameList[3];
            //find the actions selection menu wrapper control
            Control[] subBranches = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-subBranch-" + subBranchId, true);
            //get subBranch 
            SubBranch subBranch = subBranches[0] as SubBranch;

            //get current target action type from tag
            string tag = currentTarget.Tag as string;

            //set dropdown menu value to selected text
            subBranch.ChooseActionTypeValue.Text = currentTarget.Text;

            //update action type value tag as the type set in db
            subBranch.ChooseActionTypeValue.Tag = currentTarget.Tag;

            //hide all other controls in this subranch 
            SetSelectedSubBranchControl(subBranch, tag);

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }

        void WebhookControlValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);

            //generate a new textbox within the webhook label
            MaskedTextBox ControlTextbox = GenerateTextBox(currentTarget, 10, Color.FromArgb(32, 68, 85), Color.FromArgb(186, 194, 198), new Point(4, 12));
            //assign event listeners specific to the related control
            ControlTextbox.LostFocus += new EventHandler(WebhookControlTextbox_LostFocus);
            ControlTextbox.KeyDown += new KeyEventHandler(WebhookControlTextbox_EnterKeyDown);
            //select all text in the created textbox
            ControlTextbox.SelectAll();
            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();
        }

        //a method responsible of handling the loss of focus on a webhook textbox
        void WebhookControlTextbox_LostFocus(object sender, EventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);
            //set text color of parent 
            currentTarget.Parent.ForeColor = currentTarget.Text == "" || currentTarget.Text == WebhookPlaceholder ? Color.FromArgb(120, 144, 154) : Color.FromArgb(62, 96, 111);
            UpdateTextboxSelectionValue(currentTarget, "Enter URL");

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }

        //a method responsible of handling keypress on an webhook textbox
        void WebhookControlTextbox_EnterKeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);

            //check if keyboard key is equal to any of the keys below
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Escape)
            {
                //set text color of parent 
                currentTarget.Parent.ForeColor = currentTarget.Text == "" || currentTarget.Text == WebhookPlaceholder ? Color.FromArgb(120, 144, 154) : Color.FromArgb(62, 96, 111);
                //run a function to update the value of the webhook textbox label value
                UpdateTextboxSelectionValue(currentTarget, "Enter URL");

                //run function to check if the branch save button should be enabled based on the conditions
                BranchSaveValidation();
                e.Handled = true;
                e.SuppressKeyPress = true;
                //Scroll back to tab temporary bugfix
                SendKeys.Send("+{TAB}");
                FlowsContainer.VerticalScroll.Value = 0;

            }

        }

        //a method responsible of listening to set volume checkbox change
        void SetVolumeControlCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox currentTarget = (sender as CheckBox);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];

            //get the SetVolumeControlValues control
            Control[] SetVolumeControlValues = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-SetVolumeControlValue-" + subBranchId, true);


            //check if the checkbox is checked
            if (currentTarget.Checked)
            {
                CreateSetVolumeTextbox(currentTarget);

                //disabled checkbox
                currentTarget.Enabled = false;

                //add label click event listener
                SetVolumeControlValues[0].Click += new EventHandler(SetVolumeControlValue_Click);
            }
            else
            {
                //return label value to voice controlled
                SetVolumeControlValues[0].Text = SetChangeVolumePlaceholder;
                // set label color
                SetVolumeControlValues[0].ForeColor = Color.FromArgb(120, 144, 154);
                //set cursor of label to default
                SetVolumeControlValues[0].Cursor = Cursors.Default;
                //remove label click event
                SetVolumeControlValues[0].Click -= new EventHandler(SetVolumeControlValue_Click);

            }

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }

        //a method responsible of listening to set volume label click
        void SetVolumeControlValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];

            //get the SetVolumeControlCheckbox control
            Control[] SetVolumeControlCheckboxes = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-SetVolumeControlCheckbox-" + subBranchId, true);

            CreateSetVolumeTextbox(currentTarget);

            //disabled checkbox
            SetVolumeControlCheckboxes[0].Enabled = false;

        }


        void CreateSetVolumeTextbox(Control currentTarget)
        {
            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];
            //get the SetVolumeControlValues control
            Control[] SetVolumeControlValues = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-SetVolumeControlValue-" + subBranchId, true);

            //generate a new textbox within the label
            MaskedTextBox ControlTextbox = GenerateTextBox(SetVolumeControlValues[0], 8, Color.FromArgb(32, 68, 85), Color.FromArgb(186, 194, 198), new Point(4, 3), "mainBranch-" + mainBranchId + "-SetVolumeTextbox-" + subBranchId, digitsNum: "000");
            //assign event listeners specific to the related control
            ControlTextbox.LostFocus += new EventHandler(SetVolumeTextbox_LostFocus);
            ControlTextbox.KeyDown += new KeyEventHandler(SetVolumeTextbox_EnterKeyDown);
            //select all text in the created textbox
            ControlTextbox.SelectAll();

            //set cursor of label to default
            SetVolumeControlValues[0].Cursor = Cursors.Hand;
        }

        //a method responsible of handling the loss of focus on a set volume textbox
        void SetVolumeTextbox_LostFocus(object sender, EventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];

            //get the SetVolumeControlCheckbox control
            Control[] SetVolumeControlCheckboxes = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-SetVolumeControlCheckbox-" + subBranchId, true);
            //Re-enabled checkbox
            SetVolumeControlCheckboxes[0].Enabled = true;
            //remove textbox events
            currentTarget.LostFocus -= new EventHandler(SetVolumeTextbox_LostFocus);
            currentTarget.KeyDown -= new KeyEventHandler(SetVolumeTextbox_EnterKeyDown);

            currentTarget.Parent.ForeColor = currentTarget.Text == "" || currentTarget.Text == SetVolumePlaceholder ? Color.FromArgb(120, 144, 154) : Color.FromArgb(32, 68, 85);
            //run a function to update the value of the set volume textbox label value
            UpdateTextboxSelectionValue(currentTarget, SetVolumePlaceholder);

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();

        }

        //a method responsible of handling keypress on an set volume textbox
        void SetVolumeTextbox_EnterKeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];


            //check if keyboard key is equal to any of the keys below
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Escape)
            {
                //get the SetVolumeControlCheckbox control
                Control[] SetVolumeControlCheckboxes = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-SetVolumeControlCheckbox-" + subBranchId, true);
                //Re-enabled checkbox
                SetVolumeControlCheckboxes[0].Enabled = true;
                //remove textbox events
                currentTarget.LostFocus -= new EventHandler(SetVolumeTextbox_LostFocus);
                currentTarget.KeyDown -= new KeyEventHandler(SetVolumeTextbox_EnterKeyDown);
                currentTarget.Parent.ForeColor = currentTarget.Text == "" || currentTarget.Text == SetVolumePlaceholder ? Color.FromArgb(120, 144, 154) : Color.FromArgb(32, 68, 85);
                //run a function to update the value of the set volume textbox label value
                UpdateTextboxSelectionValue(currentTarget, SetVolumePlaceholder);
                //run function to check if the branch save button should be enabled based on the conditions
                BranchSaveValidation();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        //a method responsible of updating the label of the textbox parent with the textbox value
        void UpdateTextboxSelectionValue(MaskedTextBox currentTarget, string placeholder)
        {
            //check if textbox has any value
            if (currentTarget.Text.Length > 0 && currentTarget.Text != placeholder)//has value
            {
                //set the text of the textbox label to the one entered by the user in the textbox
                currentTarget.Parent.Text = currentTarget.Text;
                //remove the textbox
                currentTarget.Parent.Controls.Remove(currentTarget);
            }
            else//no value
            {
                //set the text of the textbox label to the default placeholder
                currentTarget.Parent.Text = placeholder;
                //remove the textbox
                currentTarget.Parent.Controls.Remove(currentTarget);
            }
        }

        void AudioDeviceItemValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);

            //get name of the current button clicked and split to array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //get main branch id
            string mainBranchId = currentTargetNameList[1];
            //get sub branch id
            string subBranchId = currentTargetNameList[3];
            //find the actions selection menu wrapper control
            Control[] subBranches = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-subBranch-" + subBranchId, true);
            //get subBranch 
            SubBranch subBranch = subBranches[0] as SubBranch;

            //get current target action type from tag
            string tag = currentTarget.Tag as string;

            //set dropdown menu value to selected text
            subBranch.DropDownControlValue.Text = currentTarget.Text;

            //update action type value tag as the type set in db
            subBranch.DropDownControlValue.Tag = currentTarget.Tag;

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();

        }

        void AudioDeviceDropdownMenu_Click(object sender, EventArgs e)
        {
            Control currentTarget = (sender as Control);
            //get sub branch control
            Control currentTargetSubBranch = currentTarget.Parent.Parent.Parent.Parent.Parent;

            //get the current y-axis position of the sub branch
            int currentTargetSubBranchPositionY = currentTargetSubBranch.FindForm().PointToClient(currentTargetSubBranch.Parent.PointToScreen(currentTargetSubBranch.Location)).Y - 27;
            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];

            //get the actions dropdown wrapper control
            Control[] AudioDeviceDropdownMenuWrapper = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-AudioDeviceDropdownMenuWrapper-" + subBranchId, true);

            //check if the dropdown wrapper is visible already
            if (AudioDeviceDropdownMenuWrapper[0].Visible)
            {
                //hide the dropdown menu wrapper
                AudioDeviceDropdownMenuWrapper[0].Visible = false;
            }
            else
            {
                //hides all the controls tagged with "Dropdown-Popper"
                HideAllDropdownPoppers();
                //update the position of the dropdown menu wrapper
                AudioDeviceDropdownMenuWrapper[0].Location = new Point(290, currentTargetSubBranchPositionY);
                //show the dropdown menu wrapper
                AudioDeviceDropdownMenuWrapper[0].Visible = true;
            }

        }


        //a method responsible of handling keypress on the relevant duration control value
        void DurationControlValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //set the main branch id
            string mainBranchId = currentTargetNameList[1];
            //set the sub branch id
            string subBranchId = currentTargetNameList[3];

            //get placeholder 
            string placeholder = currentTargetNameList[2].Substring(8, 2).ToLower();
            //generate a new textbox within the label
            MaskedTextBox ControlTextbox = GenerateTextBox(currentTarget, 8, Color.FromArgb(32, 68, 85), Color.White, new Point(Convert.ToInt32(currentTarget.Width / 3.2), 8), "mainBranch-" + mainBranchId + "-DurationControl" + placeholder + "TextboxValue-" + subBranchId, digitsNum: "00");
            //set textbox max length
            ControlTextbox.MaxLength = 2;
            //move padding of label so it appears hidden
            currentTarget.Padding = new Padding(0, 0, 25, 0);
            //assign event listeners specific to the related control
            ControlTextbox.LostFocus += new EventHandler(DurationControlValue_LostFocus);
            ControlTextbox.KeyDown += new KeyEventHandler(DurationControlValue_KeyDown);
            //select all text in the created textbox
            ControlTextbox.SelectAll();
            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();
        }


        //a method responsible of handling the loss of focus on duration value textbox
        void DurationControlValue_LostFocus(object sender, EventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });

            //get placeholder 
            string placeholder = currentTargetNameList[2].Substring(15, 2).ToLower();

            //move padding of label back to where it was so it's visible again
            currentTarget.Parent.Padding = new Padding(0, 0, 0, 0);

            //run a function to update the value of the set volume textbox label value
            UpdateTextboxSelectionValue(currentTarget, placeholder);

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }

        //a method responsible of handling keypress on  duration value textbox
        void DurationControlValue_KeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);

            //get the name list of the currently clicked control and split it into array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });

            //get placeholder 
            string placeholder = currentTargetNameList[2].Substring(15, 2).ToLower();

            //check if keyboard key is equal to any of the keys below
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Escape)
            {
                //move padding of label back to where it was so it's visible again
                currentTarget.Parent.Padding = new Padding(0, 0, 0, 0);

                //run a function to update the value of the set volume textbox label value
                UpdateTextboxSelectionValue(currentTarget, placeholder);

                //run function to check if the branch save button should be enabled based on the conditions
                BranchSaveValidation();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        //a method responsible of handling click on the choose file button
        void FileSelectControlChooseFileButton_Click(object sender, EventArgs e)
        {
            //update the focus of the form elsewhere
            UnfocusAll();

            Label currentTarget = (sender as Label);

            SubBranch subBranch = currentTarget.Parent.Parent.Parent.Parent.Parent as SubBranch;

            //create a new file dialog for the relevant control within the sub branch
            OpenFileDialog FileSelectControlDialog = new OpenFileDialog();
            //check if user selected a file
            if (FileSelectControlDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //get path of the file selected
                    string pathName = FileSelectControlDialog.FileName;
                    //split the path string into an array based on the dash character 
                    string[] pathNameArray = pathName.Split(new Char[] { '\\' });
                    //set file name label of control based on the file selected
                    subBranch.FileSelectControlFileName.Text = pathNameArray[pathNameArray.Length - 1];
                    //set file tag of control based on the file selected
                    subBranch.FileSelectControlFileName.Tag = pathName;
                }
                catch (System.Security.SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                //run function to check if the branch save button should be enabled based on the conditions
                BranchSaveValidation();
            }

        }


        //a method responsible of handling click on the relevant file select parameter textbox control value
        void FileSelectControlParamValue_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);

            //generate a new textbox within the label
            MaskedTextBox ControlTextbox = GenerateTextBox(currentTarget, 9, Color.FromArgb(32, 68, 85), Color.FromArgb(186, 194, 198), new Point(4, 1));
            //assign event listeners specific to the related control
            ControlTextbox.LostFocus += new EventHandler(FileSelectControlParamTextbox_LostFocus);
            ControlTextbox.KeyDown += new KeyEventHandler(FileSelectControlParamTextbox_EnterKeyDown);
            //select all text in the created textbox
            ControlTextbox.SelectAll();
            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();
        }

        //a method responsible of handling the loss of focus on the relevant file select parameter textbox 
        void FileSelectControlParamTextbox_LostFocus(object sender, EventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);
            //set text color of parent 
            currentTarget.Parent.ForeColor = currentTarget.Text == "" || currentTarget.Text == FileSelectParamsPlaceholder ? Color.FromArgb(120, 144, 154) : Color.FromArgb(62, 96, 111);
            //run a function to update the value of the textbox label value
            UpdateTextboxSelectionValue(currentTarget, FileSelectParamsPlaceholder);
            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }

        //a method responsible of handling keypress on the relevant file select parameter textbox
        void FileSelectControlParamTextbox_EnterKeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox currentTarget = (sender as MaskedTextBox);

            //check if keyboard key is equal to any of the keys below
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Escape)
            {
                //set text color of parent 
                currentTarget.Parent.ForeColor = currentTarget.Text == "" || currentTarget.Text == FileSelectParamsPlaceholder ? Color.FromArgb(120, 144, 154) : Color.FromArgb(62, 96, 111);
                //run a function to update the value of the textbox label value
                UpdateTextboxSelectionValue(currentTarget, FileSelectParamsPlaceholder);
                //run function to check if the branch save button should be enabled based on the conditions
                BranchSaveValidation();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        //a method that creates a textbox within the current target
        MaskedTextBox GenerateTextBox(Control currentTarget, int fontSize, Color fontColor, Color backColor, Point location, string textboxName = "", string digitsNum = "")
        {

            //create a new text box
            MaskedTextBox ControlTextbox = new MaskedTextBox();
            ControlTextbox.BorderStyle = BorderStyle.None;
            ControlTextbox.BackColor = backColor;
            ControlTextbox.Text = currentTarget.Text;
            ControlTextbox.AutoSize = false;
            ControlTextbox.Size = new Size(currentTarget.Width, 25);
            ControlTextbox.Padding = new Padding(0, 20, 0, 20);
            ControlTextbox.Location = location;
            ControlTextbox.ForeColor = fontColor;
            ControlTextbox.Name = textboxName;
            ControlTextbox.Font = new Font(ControlTextbox.Font.FontFamily, fontSize);
            ControlTextbox.Mask = digitsNum;
            ControlTextbox.PromptChar = Convert.ToChar(" ");

            //add new text box into clicked upon control
            currentTarget.Controls.Add(ControlTextbox);
            //set the focus on the newly created textbox
            ControlTextbox.Focus();

            return ControlTextbox;
        }

        // a method that hides all the dynamic controls and shows the selected one based on its tag in the specific subbranch 
        void SetSelectedSubBranchControl(SubBranch subBranch, string tag, dynamic subflow = null)
        {
            subBranch.FileSelectControl.Visible = false;
            subBranch.SetVolumeControl.Visible = false;
            subBranch.SetVolumeControl.Visible = false;
            subBranch.DurationControl.Visible = false;
            subBranch.WebhookControl.Visible = false;
            subBranch.DropDownControl.Visible = false;

            //split the tag name into an array based on dash seperation
            string[] taglist = tag.Split(new Char[] { '-' });

            //show the selected control in the relevant sub branch
            switch (taglist[0])
            {
                case "launch":
                    subBranch.FileSelectControl.Visible = true;
                    //check if control data should be updated based on a db subflow
                    if (subflow != null)
                    {
                        //get path of the file selected from db
                        string pathName = subflow.param1;
                        //split the path string into an array based on the dash character 
                        string[] pathNameArray = pathName.Split(new Char[] { '\\' });
                        //set file name label of control based on the file selected
                        subBranch.FileSelectControlFileName.Text = pathNameArray[pathNameArray.Length - 1];
                        //set file tag of control based on the file selected
                        subBranch.FileSelectControlFileName.Tag = pathName;
                        //set paramaters textbox of the file selected based on db 
                        subBranch.FileSelectControlParamValue.Text = subflow.param2 == "" ? FileSelectParamsPlaceholder : subflow.param2;
                        //update parameters textcolor
                        subBranch.FileSelectControlParamValue.ForeColor = subflow.param2 == "" ? Color.FromArgb(120, 144, 154) : Color.FromArgb(62, 96, 111); ;
                        //update action type value tag as the type set in db
                        subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.actiontype; ;
                    }


                    break;
                case "setvolume":
                case "changevolume":
                    subBranch.SetVolumeControl.Visible = true;
                    //check if control data should be updated based on a db subflow
                    if (subflow != null)
                    {
                        if (taglist[0] == "setvolume")
                        {
                            //set text to lavel based on db value
                            subBranch.SetVolumeControlValue.Text = Convert.ToString(subflow.param1);
                            //update action type value tag as the type set in db
                            subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.actiontype;
                        }
                        else if (taglist[0] == "changevolume")
                        {
                            //set text to lavel based on db value
                            subBranch.SetVolumeControlValue.Text = Convert.ToString(subflow.param2);
                            //update action type value tag as the type set in db
                            subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.param1;
                        }

                        //check if user selcted a non-voice controlled option
                        if (subBranch.SetVolumeControlValue.Text != SetChangeVolumePlaceholder)
                        {
                            // set label color
                            subBranch.SetVolumeControlValue.ForeColor = Color.FromArgb(32, 68, 85);
                            //set checkbox to enabeld
                            subBranch.SetVolumeControlCheckbox.Checked = true;
                            //set cursor to hand 
                            subBranch.SetVolumeControlValue.Cursor = Cursors.Hand;
                            //add label click event listener
                            subBranch.SetVolumeControlValue.Click += new EventHandler(SetVolumeControlValue_Click);
                        }
                        else
                        {
                            // set label color
                            subBranch.SetVolumeControlValue.ForeColor = Color.FromArgb(120, 144, 154);
                            //set cursor of label to default
                            subBranch.SetVolumeControlValue.Cursor = Cursors.Default;
                        }

                    }
                    break;
                case "changeaudiodevice":
                    subBranch.DropDownControl.Visible = true;
                    //check if control data should be updated based on a db subflow

                    if (subflow != null)
                    {
                        //set text to lavel based on db value
                        subBranch.DropDownControlValue.Text = Convert.ToString(subflow.param1);
                        //update action type value tag as the type set in db
                        subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.actiontype;
                    }

                    break;
                case "screencontrol":
                    //check if control data should be updated based on a db subflow
                    if (subflow != null)
                    {
                        //update action type value tag as the type set in db
                        subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.param1;
                    }
                    break;
                case "timetask":
                    subBranch.DurationControl.Visible = true;
                    //check if control data should be updated based on a db subflow
                    if (subflow != null)
                    {
                        //check which timetask is the flow related to 
                        if (subflow.param1 == "delay")
                        {
                            //get path of the file selected from db
                            string timestring = subflow.param2;
                            //split the path string into an array based on the dash character 
                            string[] timestringArray = timestring.Split(new Char[] { ':' });

                            //set hours of duration textbox based on db 
                            subBranch.DurationHhValue.Text = timestringArray[0];
                            //set minutes of duration textbox based on db 
                            subBranch.DurationMmValue.Text = timestringArray[1];
                            //set seconds of duration textbox based on db 
                            subBranch.DurationSsValue.Text = timestringArray[2];

                        }
                        else if (subflow.param1 == "scheduled")
                        {

                        }
                        //update action type value tag as the type set in db
                        subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.param1;
                    }
                    break;
                case "computertask":
                    //check if control data should be updated based on a db subflow
                    if (subflow != null)
                    {
                        //update action type value tag as the type set in db
                        subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.param1;
                    }
                    break;
                case "webhook":
                    subBranch.WebhookControl.Visible = true;
                    //check if control data should be updated based on a db subflow
                    if (subflow != null)
                    {
                        //set webhook textbox based on db 
                        subBranch.WebhookControlValue.Text = subflow.param1;
                        subBranch.ChooseActionTypeValue.Tag = subflow.actiontype + "-" + subflow.actiontype;
                    }
                    break;
            }
        }

        //a method responsible of click on the remove sub branch button
        void RemoveSubBranchButton_Click(object sender, EventArgs e)
        {
            PictureBox currentTarget = (sender as PictureBox);
            //get sub branch
            SubBranch subBranch = currentTarget.Parent.Parent as SubBranch;
            //get main branch
            MainBranch mainBranch = subBranch.Parent.Parent.Parent as MainBranch;

            //get name of the current button clicked and split to array based on the dash character
            string[] currentTargetNameList = currentTarget.Name.Split(new Char[] { '-' });
            //get main branch id
            string mainBranchId = currentTargetNameList[1];
            //get sub branch id
            string subBranchId = currentTargetNameList[3];
            //find the actions selection menu wrapper control
            Control[] ActionSelectionDropdownMenuWrapper = FlowsContainer.Controls.Find("mainBranch-" + mainBranchId + "-ActionSelectionDropdownMenuWrapper-" + subBranchId, true);
            //remove the dropdown menu wrapper control
            ActionSelectionDropdownMenuWrapper[0].Parent.Controls.Remove(ActionSelectionDropdownMenuWrapper[0]);

            //remove sub branch
            mainBranch.ActionsContainer.Controls.Remove(subBranch);

            //get the current list of actions in the main branch
            int ActionsContainerCount = mainBranch.ActionsContainer.Controls.Count;

            //Check if this is the last subBranch that was removed
            if (ActionsContainerCount == 0)
            {
                //run toggle main branch collapsible
                ToggleMainBranchCollapsible(mainBranch, false);
                //update accordion toggle button cursor to default one
                mainBranch.AccordionCollapsibleButton.Cursor = Cursors.Default;
                //update accordion toggle button image to disabled one
                mainBranch.AccordionCollapsibleButton.BackgroundImage = Image.FromFile("Images/Open_Accordion_Button_Disabled.png");
                //remove click event from the accordion toggle button
                mainBranch.AccordionCollapsibleButton.Click -= new EventHandler(AccordionCollapsibleButton_Click);
            }
            else
            {
                //update the size of the current main branch
                mainBranch.Size = new Size(mainBranch.Width, mainBranch.Height - 55);
            }

            //hides all the controls tagged with "Dropdown-Popper"
            HideAllDropdownPoppers();

            //run function to check if the branch save button should be enabled based on the conditions
            BranchSaveValidation();
        }

        //<------Form Controls Settings----->

        //Listening to AddNewFlowBtn control click whenever a user wants to add a new flow
        private void AddNewFlowBtn_Click(object sender, EventArgs e)
        {
            Label currentTarget = (sender as Label);
            string generatedGUID = Guid.NewGuid().ToString("N");

            CreateMainBranch(generatedGUID);
        }

        private void AddNewFlowBtn_MouseHover(object sender, EventArgs e)
        {
            AddMainBranchButtonTooltip.Show("Add a new branch to create a new flow and enrich your automation", AddNewFlowBtn);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
        }

        private void MainApp_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }

        private void MainApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            SettingsPage sp = new SettingsPage(); //this is the change, code for redirect  
            sp.ShowDialog();
        }

        private void AllClicks(object sender, EventArgs e)
        {
            //update the focus of the form elsewhere
            UnfocusAll();
        }

        void UnfocusAll()
        {
            FlowsContainer.Focus();
        }

        void SetFlowsContainerScroll(int newValue)
        {
            FlowsContainer.VerticalScroll.Value = newValue;
        }

        //When form loads for the first time
        private void MainApp_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (var flow in Globals.jsondb)
                {
                    CreateMainBranch(Convert.ToString(flow.id), Convert.ToString(flow.userintentvalue), Convert.ToString(flow.utterance), true, flow.actionlist, Convert.ToString(flow.intent));
                }
                SetFlowsContainerScroll(0);
            }
            catch
            {

            }





        }

    }

}
