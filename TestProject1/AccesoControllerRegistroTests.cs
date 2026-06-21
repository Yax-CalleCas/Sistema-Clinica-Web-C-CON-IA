using Moq;
using MediCita.Web.Servicios.Contrato;
using MediCita.Web.Entidades;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class AccesoControllerRegistroTests
{
    [Fact]
    public async Task Registro_CorreoDuplicado_RetornaErrorYRedirecciona()
    {
        // 1. ARRANGE
        var mockUsuarioService = new Mock<IUsuarioService>();
        var mockAdminService = new Mock<IAdminUsuariosService>();
        var mockVentaService = new Mock<IVentaService>();

        // Configuramos el mock para que cuando se intente registrar, devuelva -1 (correo duplicado)
        mockUsuarioService.Setup(s => s.RegistrarUsuario(It.IsAny<Usuario>()))
                          .ReturnsAsync(-1);

        // Mock de infraestructura necesario
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        var controller = new AccesoController(mockUsuarioService.Object, mockAdminService.Object, mockVentaService.Object)
        {
            TempData = tempData
        };

        // 2. ACT
        var resultado = await controller.Registro("Juan Perez", "12345678", "test@test.com", "Password123");

        // 3. ASSERT
        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Registro", redirectResult.ActionName);

        // Verificamos que el mensaje de error específico esté en el TempData
        Assert.Equal("El correo electrónico ya se encuentra registrado.", controller.TempData["ErrorRegistro"]);
    }

    [Fact]
    public async Task Registro_UsuarioValido_RetornaExitoYRedireccionaAlHome()
    {
        // 1. ARRANGE
        var mockUsuarioService = new Mock<IUsuarioService>();
        var mockAdminService = new Mock<IAdminUsuariosService>();
        var mockVentaService = new Mock<IVentaService>();

        // Simulamos que el servicio registra correctamente y devuelve un ID de usuario, por ejemplo, 5
        mockUsuarioService.Setup(s => s.RegistrarUsuario(It.IsAny<Usuario>()))
                          .ReturnsAsync(5);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        var controller = new AccesoController(mockUsuarioService.Object, mockAdminService.Object, mockVentaService.Object)
        {
            TempData = tempData
        };

        // 2. ACT
        var resultado = await controller.Registro("Nuevo Paciente", "12345678", "nuevo@mail.com", "Clave123");

        // 3. ASSERT
        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);

        // Verificamos que redirija al Index de Home
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        // Verificamos que el mensaje de éxito esté presente
        Assert.Equal("¡Registro exitoso! Ya puedes iniciar sesion.", controller.TempData["ExitoRegistro"]);
    }
    [Fact]
    public async Task Login_UsuarioInactivo_RetornaRedirectToIndexConMensajeDeError()
    {
        // 1. ARRANGE
        var mockUsuarioService = new Mock<IUsuarioService>();
        var mockAdminService = new Mock<IAdminUsuariosService>();
        var mockVentaService = new Mock<IVentaService>();

        // Creamos un usuario que existe pero está inactivo (Activo = false)
        var usuarioInactivo = new Usuario
        {
            IdUsuario = 1,
            NombreCompleto = "Usuario Bloqueado",
            Activo = false // <--- La clave de la prueba
        };

        mockUsuarioService.Setup(s => s.ValidarUsuario(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync(usuarioInactivo);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        var controller = new AccesoController(mockUsuarioService.Object, mockAdminService.Object, mockVentaService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = tempData
        };

        // 2. ACT
        var resultado = await controller.Login("bloqueado@test.com", "123456");

        // 3. ASSERT
        var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);

        // Verificamos que redirige a Home/Index
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        // Verificamos el mensaje de error específico
        Assert.Equal("Tu cuenta está inactiva. Contacta con soporte.", controller.TempData["ErrorLogin"]);
    }
}