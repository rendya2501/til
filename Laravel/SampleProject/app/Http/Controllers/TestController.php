<?php

namespace App\Http\Controllers;

use App\Services\TestService;

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

    public function index2()
    {
        $this->test->hoge();
    }
}
