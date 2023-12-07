package com.senabo.common.api;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

@Getter
@RequiredArgsConstructor
public class ApiResponse<T> {
    private final ApiStatus status;
    private final String message;
    private final T data;

    public static <T> ApiResponse<T> success(String message, T data) {
        return new ApiResponse<>(ApiStatus.SUCCESS, message, data);
    }

    public static <T> ApiResponse<T> fail(String message, T data) {
        return new ApiResponse<>(ApiStatus.FAIL, message, data);
    }
}
