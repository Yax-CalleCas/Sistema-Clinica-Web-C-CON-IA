using MediCita.Web.Entidades;
using MediCita.Web.Servicios.Contrato;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

public class AccesoControllerTests
{
    [Fact]
    public async Task Login_CredencialesInvalidas_RetornaRedirectToIndex()
    {
        // 1. ARRANGE
        var mockUsuarioService = new Mock<IUsuarioService>();
        var mockAdminService = new Mock<IAdminUsuariosService>();
        var mockVentaService = new Mock<IVentaService>();

        // Simulamos un escenario de credenciales incorrectas
        mockUsuarioService.Setup(s => s.ValidarUsuario(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync((Usuario?)null);

        // Creamos un HttpContext simulado completo
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new Mock<ISession>().Object;

        // Mock de TempData (esto evita el error de referencia nula al intentar guardar el mensaje de error)
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        var controller = new AccesoController(mockUsuarioService.Object, mockAdminService.Object, mockVentaService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = tempData // Asignamos el TempData mockeado
        };

        // 2. ACT
        var resultado = await controller.Login("invalido@test.com", "123456");

        // 3. ASSERT
        // Verificamos que sea un RedirectToActionResult
        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);

        // Verificamos que redirija a "Index" de "Home" como dice tu lógica
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        // Verificamos que el mensaje de error se guardó correctamente
        Assert.Equal("Credenciales incorrectas.", controller.TempData["ErrorLogin"]);
    }
}