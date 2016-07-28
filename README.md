# Thread Pool + Task Testing

**Note:** This is for before + after testing, rather than a rigorous algorithm choice comparision.

Supporting investigation on PR [https://github.com/dotnet/coreclr/pull/5943](https://github.com/dotnet/coreclr/pull/5943#issuecomment-235444173)

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
QUWI No Queues             11.065 M    10.939 M    10.258 M    12.078 M    12.581 M
- Depth    2                9.903 M     8.503 M     9.129 M     9.345 M     9.706 M
- Depth   16               10.144 M     9.990 M    10.221 M    10.138 M    10.046 M
- Depth   64                9.911 M    10.285 M    10.337 M    10.403 M    10.360 M
- Depth  512               10.066 M    10.138 M    10.434 M    10.408 M    10.309 M
                       
SubTask Chain Return      557.770 k   997.956 k     4.118 M     4.265 M     4.433 M
- Depth    2              551.729 k     1.143 M     4.101 M     4.600 M     4.817 M
- Depth   16              323.886 k   796.379 k     3.834 M     4.121 M     5.469 M
- Depth   64              469.299 k     1.185 M     2.585 M     4.922 M     5.278 M
- Depth  512              361.030 k     1.519 M     3.207 M     4.480 M     5.580 M
                       
SubTask Chain Awaited     479.992 k   834.897 k     2.579 M     2.533 M     2.841 M
- Depth    2              472.915 k   848.899 k     2.476 M     2.889 M     2.949 M
- Depth   16              479.326 k     1.150 M     2.783 M     2.715 M     3.288 M
- Depth   64              532.761 k     1.244 M     2.719 M     3.260 M     3.339 M
- Depth  512              488.210 k   941.299 k     2.713 M     2.890 M     2.999 M
                       
SubTask Fanout Awaited    242.850 k   465.681 k     1.557 M     1.573 M     1.594 M
- Depth    2              308.711 k   914.097 k     1.779 M     1.948 M     1.988 M
- Depth   16                1.546 M     2.448 M     2.733 M     2.755 M     2.668 M
- Depth   64                1.950 M     2.604 M     2.883 M     2.899 M     2.877 M
- Depth  512                2.221 M     2.732 M     2.991 M     2.979 M     2.883 M
                       
Continuation Chain        122.725 k   468.872 k     1.545 M     1.692 M     1.690 M
- Depth    2              215.449 k   487.005 k     2.534 M     2.797 M     2.904 M
- Depth   16              353.085 k     1.499 M     7.306 M     7.427 M     7.338 M
- Depth   64              773.903 k     2.700 M     8.688 M     8.322 M     8.626 M
- Depth  512              738.323 k     3.582 M     9.351 M     9.348 M     8.937 M
                       
Continuation Fanout       115.174 k   320.221 k     1.216 M     1.247 M     1.248 M
- Depth    2              209.837 k   711.798 k     1.882 M     1.837 M     1.954 M
- Depth   16              963.554 k     2.907 M     4.473 M     4.468 M     4.254 M
- Depth   64                1.884 M     4.212 M     5.373 M     5.225 M     5.301 M
- Depth  512                3.645 M     3.772 M     5.646 M     5.601 M     5.611 M
                       
Yield Chain Awaited       808.624 k     1.881 M     4.278 M     4.406 M     4.504 M
- Depth    2                1.153 M     2.357 M     4.805 M     4.958 M     5.041 M
- Depth   16                1.777 M     4.212 M     5.926 M     5.978 M     5.987 M
- Depth   64                2.301 M     4.715 M     6.206 M     6.372 M     5.720 M
- Depth  512                2.545 M     5.325 M     6.185 M     5.936 M     5.462 M
                       
Async Chain Awaited       592.889 k   897.167 k     3.081 M     3.439 M     3.518 M
- Depth    2                1.067 M     1.388 M     4.764 M     4.877 M     5.192 M
- Depth   16                2.040 M     3.782 M     7.985 M     7.971 M     7.937 M
- Depth   64                2.099 M     4.297 M     8.444 M     8.477 M     8.566 M
- Depth  512                2.221 M     4.565 M     6.585 M     6.914 M     7.995 M
                       
Async Chain Return        547.907 k   936.226 k     3.588 M     3.379 M     3.602 M
- Depth    2                1.258 M     1.698 M     7.458 M     7.264 M     6.937 M
- Depth   16               10.433 M    11.505 M    58.364 M    58.180 M    53.440 M
- Depth   64               41.913 M    30.086 M   225.068 M   186.112 M   181.087 M
- Depth  512              293.857 M   163.627 M     1.229 B     1.118 B   571.817 M
                       
Sync Chain Awaited         27.920 M    55.827 M   103.603 M   106.069 M   101.552 M
- Depth    2               29.483 M    58.517 M   106.378 M   105.173 M   104.088 M
- Depth   16               21.292 M    42.062 M    72.892 M    76.854 M    75.315 M
- Depth   64               19.241 M    38.882 M    71.613 M    71.951 M    69.899 M
- Depth  512               18.582 M    38.044 M    69.779 M    70.000 M    68.339 M
                       
CachedTask Chain Await     23.531 M    47.935 M    84.763 M    87.384 M    83.791 M
- Depth    2               26.462 M    53.284 M    97.192 M    98.533 M    92.412 M
- Depth   16               20.721 M    40.831 M    74.325 M    57.111 M    73.544 M
- Depth   64               18.701 M    38.360 M    70.156 M    70.340 M    68.804 M
- Depth  512               18.642 M    37.742 M    67.888 M    68.270 M    66.448 M
                       
CachedTask Chain Check    113.693 M   222.079 M   408.407 M   413.653 M   344.156 M
- Depth    2              137.873 M   269.402 M   507.274 M   466.158 M   380.603 M
- Depth   16              152.494 M   299.149 M   551.221 M   548.889 M   457.095 M
- Depth   64               89.914 M   178.759 M   327.422 M   294.032 M   271.894 M
- Depth  512               80.811 M   162.357 M   303.450 M   300.472 M   262.223 M
                       
CachedTask Chain Return   121.164 M   233.218 M   455.633 M   441.194 M   379.644 M
- Depth    2              199.925 M   392.126 M   764.736 M   720.552 M   572.742 M
- Depth   16              579.875 M     1.089 B     1.999 B     1.877 B     1.034 B
- Depth   64              942.659 M     1.715 B     3.171 B     2.628 B     1.303 B
- Depth  512                1.443 B     2.556 B     4.031 B     3.256 B     1.321 B
                       
QUWI Local Queues          10.847 M    11.131 M     9.960 M    11.992 M    12.668 M
- Depth    2                9.732 M     8.540 M     9.135 M     9.636 M     9.795 M
- Depth   16                9.924 M     9.984 M    10.036 M    10.168 M    10.111 M
- Depth   64               10.374 M    10.372 M    10.424 M    10.277 M    10.342 M
- Depth  512               10.213 M    10.468 M    10.427 M    10.407 M    10.385 M
```
