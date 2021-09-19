<?php

namespace App\Http\Controllers;

use App\Services\TestService;
use Illuminate\Support\Collection;

class TestController extends Controller
{
    private $test;
    public function __construct(TestService $test_service)
    {
        $this->test = $test_service;
    }

    public function index(TestService $test_service)
    {
        $this->test->hoge();
        $test_service->hoge();
    }

    // public function index()
    // {
    //     // $collection = collect([
    //     //     ['product' => 'Desk', 'price' => 200],
    //     //     ['product' => 'Chair', 'price' => 100],
    //     //     ['product' => 'Bookcase', 'price' => 150],
    //     //     ['product' => 'Door', 'price' => 100],
    //     // ]);
    //     // $filtered = $collection->where('price', 20000);
    //     // var_dump($filtered->all());

    //     // $test = collect()->first();
    //     // var_dump($test);

    //     // foreach(collect()->first() as $a){

    //     // }
    //     return "test";
    // }
}
