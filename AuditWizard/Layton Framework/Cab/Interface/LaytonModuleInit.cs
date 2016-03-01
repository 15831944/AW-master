using System;
using Microsoft.Practices.CompositeUI;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Resources;
using Layton.AuditWizard.DataAccess;

namespace Layton.Cab.Interface
{
    public abstract class LaytonModuleInit : ModuleInit
    {
        private WorkItem rootWorkItem;
        private LaytonWorkItem moduleWorkItem;
        private Assembly childAssembly;
        private string assemblyFileName;

        public LaytonModuleInit(WorkItem workItem)
        {
            this.rootWorkItem = workItem;
            childAssembly = this.GetType().Assembly;
            assemblyFileName = childAssembly.GetName().Name + ".dll";
        }

        public WorkItem RootWorkItem
        {
            get { return rootWorkItem; }
        }

        public override void Load()
        {
            // load the modules applicationSettings to retrieve the CAB objects to create
            KeyValueConfigurationCollection moduleSettings = GetAssemblySettings(assemblyFileName);

            // Load the Module's main objects and properties (WorkItems, Views, Title, etc)
            LoadRequiredObjects(moduleSettings);
            LoadWorkItemProperties(moduleSettings);
            LoadAdditionalObjects(moduleSettings);

            LaytonWorkItemController wic = (LaytonWorkItemController)moduleWorkItem.Items[ControllerNames.WorkItemController];
            wic.Initialize();
        }

        private void LoadRequiredObjects(KeyValueConfigurationCollection moduleSettings)
        { 
            try
            {
                // ----------------------------------------------------
                //     First load the MainWorkItem for this module
                // ----------------------------------------------------
                string workItemName = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                      moduleSettings[SettingsKeys.MainWorkItem].Value;
                
                // add the new MainWorkItem to the RootWorkItem
                moduleWorkItem = RootWorkItem.WorkItems.AddNew(childAssembly.GetType(workItemName), moduleSettings[SettingsKeys.MainWorkItem].Value) as LaytonWorkItem;
                moduleSettings.Remove(SettingsKeys.MainWorkItem);

                // ----------------------------------------------------
                //     Load the WorkItemController into the WorkItem
                // ----------------------------------------------------
                string controllerName = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                        moduleSettings[SettingsKeys.WorkItemController].Value;

                // add the new WorkItemController into the WorkItem's Items collection
                moduleWorkItem.Items.AddNew(childAssembly.GetType(controllerName), ControllerNames.WorkItemController);
                moduleSettings.Remove(SettingsKeys.WorkItemController);

                // ----------------------------------------------------
                //     Load the ToolbarsController into the WorkItem
                // ----------------------------------------------------
                string toolbarsName = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                      moduleSettings[SettingsKeys.ToolbarsController].Value;

                // add the new ToolbarsController into the WorkItem's Items collection
                moduleWorkItem.Items.AddNew(childAssembly.GetType(toolbarsName), ControllerNames.ToolbarsController);
                moduleSettings.Remove(SettingsKeys.ToolbarsController);

                // ----------------------------------------------------
                //     Load the MainExplorerView into the WorkItem
                // ----------------------------------------------------
                string explorerViewName = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                          moduleSettings[SettingsKeys.MainExplorerView].Value;

                // add the new MainExplorerView to the WorkItem's SmartParts collection
                moduleWorkItem.Items.AddNew(childAssembly.GetType(explorerViewName), ViewNames.MainExplorerView);
                moduleSettings.Remove(SettingsKeys.MainExplorerView);

                // ----------------------------------------------------
                //     Load the MainTabView into the WorkItem
                // ----------------------------------------------------
                string tabViewName = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                     moduleSettings[SettingsKeys.MainTabView].Value;

                // add the new MainTabView to the WorkItem's SmartParts collection
                moduleWorkItem.Items.AddNew(childAssembly.GetType(tabViewName), ViewNames.MainTabView);
                moduleSettings.Remove(SettingsKeys.MainTabView);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error loading Required CAB Module objects for Assembly: {0}{1}{2}", assemblyFileName, Environment.NewLine, e.Message), e);
            }
        }

        public void LoadAdditionalObjects(KeyValueConfigurationCollection moduleSettings)
        {
            try
            {
                foreach (string key in moduleSettings.AllKeys)
                {
                    switch (key)
                    {
                        case SettingsKeys.SubWorkItems:
                            // ----------------------------------------------------
                            //     Load the SubWorkItems into the WorkItem
                            // ----------------------------------------------------
                            string subWorkItemsName = moduleSettings[SettingsKeys.SubWorkItems].Value;
                            if (subWorkItemsName != null)
                            {
                                // separate the value into the Types of WorkItems
                                string[] subWorkItems = subWorkItemsName.Split(',');
                                foreach (string subWorkItem in subWorkItems)
                                {
                                    moduleWorkItem.WorkItems.AddNew(childAssembly.GetType(moduleSettings[SettingsKeys.Namespace].Value + "." + subWorkItem), subWorkItem);
                                }
                            }
                            break;
                        case SettingsKeys.Services:
                            // ----------------------------------------------------
                            //     Load the Services into the WorkItem
                            // ----------------------------------------------------
                            string servicesName = moduleSettings[SettingsKeys.Services].Value;
                            if (servicesName != null)
                            {
                                // separate the value into the Types of Services
                                string[] services = servicesName.Split(',');
                                foreach (string service in services)
                                {
                                    moduleWorkItem.Services.AddNew(childAssembly.GetType(moduleSettings[SettingsKeys.Namespace].Value + "." + service));
                                }
                            }
                            break;
                        case SettingsKeys.SettingsTabView:
                            // ----------------------------------------------------
                            //     Load the SettingsTabView into the WorkItem
                            // ----------------------------------------------------
                            string settingsViewName = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                                      moduleSettings[SettingsKeys.SettingsTabView].Value;
                            //if (settingsViewName != null)
                            //{
                            //    moduleWorkItem.Items.AddNew(childAssembly.GetType(settingsViewName), ViewNames.SettingsTabView);
                            //}
                            break;
                        default:
                            // --------------------------------------------------------------
                            //     Non-Standard object - add to WorkItem's Item collection
                            // --------------------------------------------------------------
                            try
                            {
                                string unknownItem = moduleSettings[SettingsKeys.Namespace].Value + "." +
                                                     moduleSettings[key].Value;
                                if (unknownItem != null)
                                {
                                    moduleWorkItem.Items.AddNew(childAssembly.GetType(unknownItem), moduleSettings[key].Value);
                                }
                            }
                            catch
                            {
                                // unknown setting or object....
                            }
                            break;
                    }
                    if (key != SettingsKeys.Namespace)
                    {
                        moduleSettings.Remove(key);
                    }
                }
            }
            catch (Exception e)
            {
                new Exception(String.Format("Error loading Optional CAB Module objects for Assembly: {0}{1}{2}", assemblyFileName, Environment.NewLine, e.Message), e);
            }
        }

        private void LoadWorkItemProperties(KeyValueConfigurationCollection moduleSettings)
        {
            // ----------------------------------------------------
            //     Set the value of this WorkItem's title
            // ----------------------------------------------------
            string title = moduleSettings[SettingsKeys.Title].Value;
            if (title == null)
            {
                throw new Exception(String.Format("The CAB assembly, {0}, is missing the following setting in it's configuration file:  {1}", assemblyFileName, SettingsKeys.Title));
            }
            // set the value to the LaytonWorkItem.Title
            moduleWorkItem.Title = title;
            moduleSettings.Remove(SettingsKeys.Title);

            // ----------------------------------------------------
            //     Set the value of this WorkItem's description
            // ----------------------------------------------------
            string description = moduleSettings[SettingsKeys.Description].Value;
            if (description == null)
            {
                throw new Exception(String.Format("The CAB assembly, {0}, is missing the following setting in it's configuration file:  {1}", assemblyFileName, SettingsKeys.Description));
            }
            // set the value to the LaytonWorkItem.Description
            moduleWorkItem.Description = description;
            moduleSettings.Remove(SettingsKeys.Description);

            // ----------------------------------------------------
            //     Set the value of this WorkItem's Image
            // ----------------------------------------------------
            string imageName = moduleSettings[SettingsKeys.Image].Value;
            string namespaceName = moduleSettings[SettingsKeys.Namespace].Value;
            if (imageName == null)
            {
                throw new Exception(String.Format("The CAB assembly, {0}, is missing the following setting in it's configuration file:  {1}", assemblyFileName, SettingsKeys.Image));
            }
            if (namespaceName == null)
            {
                throw new Exception(String.Format("The CAB assembly, {0}, is missing the following setting in it's configuration file:  {1}", assemblyFileName, SettingsKeys.Namespace));
            }

            // set the value to the LaytonWorkItem.Image
            ResourceManager resourceManager = new ResourceManager(namespaceName+".Properties.Resources", childAssembly);
            moduleWorkItem.Image = (System.Drawing.Image)resourceManager.GetObject(imageName);
            moduleSettings.Remove(SettingsKeys.Image);
        }

        private KeyValueConfigurationCollection GetAssemblySettings(string assemblyPath)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyPath);

            XmlDocument dom = new XmlDocument();
            dom.Load(config.FilePath);

            //UserSettings and ApplicationSettings
            KeyValueConfigurationCollection returnList = new KeyValueConfigurationCollection();

            string[] settingsTypes = { "applicationSettings", "userSettings" };
            foreach (string settingType in settingsTypes)
            {
                XmlNode node = dom.SelectSingleNode("//configuration//" + settingType);
                if (node != null)
                {
                    try
                    {
                        if (node.HasChildNodes)
                        {
                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                if (childNode.HasChildNodes)
                                {
                                    foreach (XmlNode settingNode in childNode.ChildNodes)
                                    {
                                        if (settingNode != null)//the Settings node
                                        {
                                            if (settingNode.Attributes.Count > 0) //there should be at least one attribute
                                                returnList.Add(settingNode.Attributes[0].Value, settingNode.InnerText);//the setting name and the setting/value
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch { throw; }
                }
            }
            return returnList;
        }

        // Get the value of the Assembly ApplicationSetting or UserSetting from the key.
        // If the key does not have a value then return the default supplied by the caller.

        private string GetAssemblySettingOrDefault(string assemblyPath, string assemblySettingKey, string assemblySettingDefaultValue)
        {
            string result = assemblySettingDefaultValue;
            KeyValueConfigurationCollection settings = GetAssemblySettings(assemblyPath);
            if (settings != null)
            {
                KeyValueConfigurationElement key = settings[assemblySettingKey];
                if (key != null)
                    result = key.Value;
            }
            return result;
        }
    }
}
