using System.Web;
using System.Web.Optimization;

namespace WebApplication2
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jQuery/jquery-{version}.js",
                        "~/Scripts/jQuery/jquery.slimscroll.js"
                        /*"~/Scripts/jQuery/jquery-ui-1.8.11.js",
                        "~/Scripts/jQuery/jquery.layout-1.4.3.js"*/));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jQuery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/aggrid").Include(
                      "~/Scripts/ag-grid.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      //"~/Content/site.css",
                      "~/Content/ng-table.css",
                      "~/Content/font-awesome.min.css",
                                            //"~/Content/angular-material.css",
                                            "~/Content/bootstrap.css",
                      "~/Content/SidebarTransitions/css/component.css",
                      "~/Content/SidebarTransitions/css/demo.css",
                      //"~/Content/SidebarTransitions/css/normalize.css",
                      "~/Content/SidebarTransitions/css/icons.css",
                      "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angularJS").Include(
                "~/Scripts/angular.js"
                     //"~/Scripts/angular-material/angular-material.js",
                     //"~/Scripts/angular-aria/angular-aria.js",
                     //"~/Script/angular-animate/angular-animate.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/customJS").Include(
                                 "~/Scripts/ConfigToolScripts/Module.js",
                                 "~/Scripts/ConfigToolScripts/Service.js",
                                 "~/Scripts/ConfigToolScripts/Controller.js",
                                 "~/Scripts/ConfigToolScripts/WizardController.js",
                                 "~/Scripts/ConfigToolScripts/GridController.js"));

            bundles.Add(new ScriptBundle("~/bundles/JSplugins").Include(
                                     "~/Scripts/fastclick.js",
                                     "~/Content/SidebarTransitions/js/classie.js",
                                     "~/Content/SidebarTransitions/js/sidebarEffects.js"));
        }
    }
}
