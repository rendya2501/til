using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISample;

public class Sample3
{
    static void Main(string[] args)
    {
        var instance = new CurrentSlipController(new CurrentSlipWorkerService());
        instance.FormatSlipData(1);
    }
}

public interface ICurrentSlipWorkerService
{
    void FormatSlipData(int data, bool mode = false);
}

public class CurrentSlipController
{
    protected ICurrentSlipWorkerService currentSlip;

    public CurrentSlipController(ICurrentSlipWorkerService currentSlip)
    {
        this.currentSlip = currentSlip;
    }

    public void FormatSlipData(int data)
    {
        this.currentSlip.FormatSlipData(data, true);
    }
}

public class CurrentSlipWorkerService : ICurrentSlipWorkerService
{
    public void FormatSlipData(int data, bool mode = false)
    {
        Console.WriteLine(data.ToString() + mode.ToString());
    }

    public void FormatSlipData(int data)
    {
        Console.WriteLine(data);
    }
}

