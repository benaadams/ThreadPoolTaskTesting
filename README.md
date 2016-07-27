# Thread Pool + Task Testing

To support investigation on PR [https://github.com/dotnet/coreclr/pull/5943](https://github.com/dotnet/coreclr/pull/5943#issuecomment-235444173)

To run

Install [.NET Core](https://dot.net/)

In the project directory enter:

`dotnet run -c Release`

Output
```
Testing 2,621,440 calls, with GCs after 262,144 calls.
Operations per second on 4 Cores
                                                                       Parallelism
                            Serial          2x         16x         64x        512x
QUWI No Queues            11.341 M    11.174 M    10.152 M    11.781 M    12.343 M
- Depth    2               9.693 M     8.577 M     9.161 M     9.611 M     9.820 M
- Depth   16               9.843 M     9.975 M    10.215 M    10.185 M    10.193 M
- Depth   64              10.212 M    10.361 M    10.344 M    10.275 M    10.187 M
- Depth  512              10.043 M    10.564 M    10.346 M    10.379 M    10.262 M

SubTasks                 574.384 k     1.074 M     3.925 M     3.786 M     4.059 M
- Depth    2             481.115 k     1.235 M     3.764 M     4.307 M     4.376 M
- Depth   16             281.681 k     1.054 M     2.196 M     3.694 M     4.601 M
- Depth   64             357.318 k     1.542 M     3.332 M     3.548 M     4.464 M
- Depth  512             292.159 k     1.386 M     4.184 M     4.770 M     5.010 M

SubTasks Awaited         434.887 k   748.854 k     2.325 M     2.367 M     2.467 M
- Depth    2             415.744 k   600.544 k     2.227 M     2.415 M     2.687 M
- Depth   16             378.693 k   763.114 k     2.496 M     2.715 M     2.891 M
- Depth   64             443.539 k   884.953 k     2.516 M     2.909 M     2.900 M
- Depth  512             501.711 k   769.023 k     2.557 M     2.530 M     2.749 M

QUWI Local Queues         11.488 M    11.030 M    10.052 M    12.056 M    12.653 M
- Depth    2               9.872 M     8.931 M     9.237 M     9.596 M     9.752 M
- Depth   16              10.307 M    10.046 M    10.144 M    10.177 M    10.133 M
- Depth   64              10.219 M    10.241 M    10.382 M    10.343 M    10.206 M
- Depth  512              10.221 M    10.278 M    10.355 M    10.426 M    10.399 M

Yielding Await           690.483 k     1.453 M     3.996 M     3.978 M     4.145 M
- Depth    2               1.046 M     2.291 M     5.418 M     5.231 M     5.371 M
- Depth   16               2.892 M     6.241 M     8.013 M     7.701 M     7.514 M
- Depth   64               3.534 M     6.442 M     7.756 M     8.239 M     8.216 M
- Depth  512               4.095 M     8.082 M     4.013 M     8.205 M     7.461 M

Async Awaited            589.227 k   837.691 k     3.116 M     3.267 M     3.405 M
- Depth    2             966.724 k     1.462 M     4.292 M     4.625 M     4.800 M
- Depth   16               1.822 M     3.702 M     7.929 M     8.070 M     7.890 M
- Depth   64               2.023 M     4.261 M     8.501 M     8.494 M     8.368 M
- Depth  512               2.205 M     4.481 M     6.615 M     6.473 M     8.280 M

Async PassThrough        544.464 k   837.542 k     2.882 M     2.994 M     3.313 M
- Depth    2               1.118 M     1.519 M     6.179 M     6.482 M     6.475 M
- Depth   16               6.413 M    10.819 M    46.852 M    50.872 M    48.595 M
- Depth   64              36.737 M    35.197 M   199.140 M   193.693 M   154.588 M
- Depth  512             249.680 M   172.013 M     1.071 B   931.372 M   537.775 M

Completed Awaited         23.509 M    46.841 M    84.602 M    84.865 M    82.628 M
- Depth    2              25.976 M    51.338 M    96.914 M    98.113 M    94.354 M
- Depth   16              20.073 M    40.692 M    74.357 M    73.212 M    72.994 M
- Depth   64              18.895 M    37.712 M    68.149 M    70.442 M    68.025 M
- Depth  512              18.814 M    37.104 M    68.433 M    66.401 M    66.533 M

CachedTask Awaited        23.695 M    48.631 M    88.577 M    87.729 M    86.142 M
- Depth    2              25.947 M    52.404 M    96.651 M    98.538 M    91.645 M
- Depth   16              20.278 M    40.970 M    71.835 M    75.324 M    71.259 M
- Depth   64              19.130 M    38.238 M    71.216 M    70.784 M    66.113 M
- Depth  512              18.269 M    37.174 M    65.379 M    66.674 M    65.857 M

CachedTask CheckAwait    113.468 M   224.028 M   420.601 M   427.425 M   358.689 M
- Depth    2             137.860 M   267.923 M   497.502 M   473.278 M   390.368 M
- Depth   16             153.004 M   301.291 M   483.723 M   565.575 M   423.393 M
- Depth   64              89.841 M   177.891 M   342.327 M   334.670 M   268.282 M
- Depth  512              81.141 M   161.585 M   276.226 M   285.728 M   249.839 M

CachedTask PassThrough   117.638 M   233.357 M   380.426 M   444.643 M   355.503 M
- Depth    2             198.782 M   390.799 M   694.145 M   647.941 M   490.126 M
- Depth   16             570.498 M     1.087 B     1.872 B     1.773 B   904.662 M
- Depth   64             947.942 M     1.776 B     2.764 B     2.412 B     1.095 B
- Depth  512               1.427 B     2.616 B     3.766 B     3.236 B     1.220 B
```
