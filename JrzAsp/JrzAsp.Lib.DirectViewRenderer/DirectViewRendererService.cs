using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JrzAsp.Lib.DirectViewRenderer {
    /// <summary>
    ///     This class encapsulates the mechanism to render a view directly without undergoing all
    ///     MVC lifecycle.
    /// </summary>
    public static class DirectViewRendererService {
        /// <summary>
        ///     Render view to string
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderViewToString(this ControllerContext context,
            string viewPath,
            object model = null,
            bool partial = false) {

            context.Controller.ViewData.Model = model;

            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            viewEngineResult = partial
                ? ViewEngines.Engines.FindPartialView(context, viewPath)
                : ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null) {
                throw new FileNotFoundException(
                    $"{typeof(DirectViewRendererService)} can't find view engine for '{viewPath}'.");
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            if (view == null) {
                throw new FileNotFoundException(
                    $"{typeof(DirectViewRendererService)} can't find view for '{viewPath}'.\r\n" +
                    $"Searched locations:\r\n- {string.Join("\r\n- ", viewEngineResult.SearchedLocations)}"
                );
            }

            string result = null;

            using (var sw = new StringWriter()) {
                var ctx = new ViewContext(context, view,
                    context.Controller.ViewData,
                    context.Controller.TempData,
                    sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        /// <summary>
        ///     Render view to string
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderViewToString(this Controller controller,
            string viewPath,
            object model = null,
            bool partial = false) {
            return RenderViewToString(controller.ControllerContext, viewPath, model, partial);
        }

        /// <summary>
        ///     Render view to string using a throwaway controller
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderViewToString(string viewPath,
            object model = null,
            bool partial = false) {
            var ctrl = CreateController<DummyController>();
            return RenderViewToString(ctrl, viewPath, model, partial);
        }

        /// <summary>
        ///     Render view to string using a throwaway controller
        /// </summary>
        /// <param name="routeData"></param>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderViewToString(RouteData routeData, string viewPath,
            object model = null,
            bool partial = false) {
            var ctrl = CreateController<DummyController>();
            return RenderViewToString(ctrl, viewPath, model, partial);
        }

        /// <summary>
        ///     Create an MVC controller directly
        /// </summary>
        /// <typeparam name="TController">The type of the controller</typeparam>
        /// <param name="routeData">route data</param>
        /// <returns>controller object</returns>
        public static TController CreateController<TController>(RouteData routeData = null)
            where TController : Controller, new() {
            // create a disconnected controller instance
            var controller = new TController();

            // get context wrapper from HttpContext if available
            HttpContextBase wrapper;
            if (HttpContext.Current != null) {
                wrapper = new HttpContextWrapper(HttpContext.Current);
            } else {
                throw new InvalidOperationException(
                    "Can't create Controller Context if no active HttpContext instance available.");
            }

            if (routeData == null) {
                routeData = new RouteData();
            }

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") &&
                !routeData.Values.ContainsKey("Controller")) {

                var ctrlVal = controller.GetType().Name.Replace("Controller", "");
                routeData.Values.Add("controller", ctrlVal);
            }

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }

        /// <summary>
        ///     Create a dummy MVC controller directly
        /// </summary>
        /// <param name="routeData">route data</param>
        /// <returns>controller object</returns>
        public static Controller CreateController(RouteData routeData = null) {
            return CreateController<DummyController>(routeData);
        }

        /// <summary>
        ///     A generic controller to be used to render cshtml directly using its ControllerContext
        /// </summary>
        internal class DummyController : Controller { }
    }
}