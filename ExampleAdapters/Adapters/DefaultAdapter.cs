using System;
using System.Collections.Generic;
using BITE.Server.Plugins.Attributes;

namespace ExampleAdapters.Adapters {
   public class DefaultAdapter {
      public object check_login_status() {
         return new {
            loggedIn = true,
            user = "jcarroll",
            url = "http://www.myUrl.com/login"
         };
      }

      [BiteMethod("get_templates")]
      public object get_templates() {
         return new List<BugTemplate> {
            //new BugTemplate {
            //   id = "CDB28DF69278448AA3E2590217C984F0",
            //   name = "Default Template",
            //   urls = new List<string>(),
            //   project = "WUI",
            //   backendProject = "WUI",
            //   backendProvider = "MyFakeProvider",
            //   selectorText = "Bug",
            //   noteText = "It's a bug yo!",
            //   displayOrder = 0
            //}
         };
      }
   }

   public class BugTemplate {
      // A unique string identifier for this template.
      public string id { get; set; }
      
      // A human-readable name for this template.
      public string name { get; set; }
      
      // A list of urls that this template should be used for.
      public List<String> urls { get; set; }
      
      // The human-readable project that this template is associated with.
      public String project { get; set; }
      
      // An identifier for the project that is compatable with the backend provider.
      public String backendProject { get; set; }
      
      // The issue tracking system that this template is associated with.
      public String backendProvider { get; set; }
      
      // Text that should appear when the user is asked to pick a
      //      template, under 'What kind of problem are you reporting?'
      public String selectorText { get; set; }

      // Text that should appear in the notes field.
      public String noteText { get; set; }
      
      // An integer declaring the relative position where this
      //      template should be displayed in lists. Higher numbers are displayed after lower numbers.
      public Int32 displayOrder { get; set; }
   }
}