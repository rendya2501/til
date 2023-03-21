using Microsoft.AspNetCore.Mvc;
using TimeToLive.Services;

namespace TimeToLive.Controllers;

[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    private readonly IMySingletonService _mySingletonService;
    private readonly IMyScopedService _myScopedService1;
    private readonly IMyScopedService _myScopedService2;
    private readonly IMyTransientService _myTransientService1;
    private readonly IMyTransientService _myTransientService2;

    public MyController(
        IMySingletonService mySingletonService,
        IMyScopedService myScopedService1,
        IMyScopedService myScopedService2,
        IMyTransientService myTransientService1,
        IMyTransientService myTransientService2)
    {
        _mySingletonService = mySingletonService;
        _myScopedService1 = myScopedService1;
        _myScopedService2 = myScopedService2;
        _myTransientService1 = myTransientService1;
        _myTransientService2 = myTransientService2;
    }

    [HttpGet]
    public IActionResult GetInstanceAddresses()
    {
        return Ok(new
        {
            SingletonAddress = _mySingletonService.GetInstanceAddress(),
            ScopedAddress1 = _myScopedService1.GetInstanceAddress(),
            ScopedAddress2 = _myScopedService2.GetInstanceAddress(),
            TransientAddress1 = _myTransientService1.GetInstanceAddress(),
            TransientAddress2 = _myTransientService2.GetInstanceAddress()
        });
    }
}
