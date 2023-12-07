package com.senabo.exception.dto;

import com.senabo.exception.message.ExceptionMessage;
import lombok.Builder;
import lombok.Data;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

@Data
@Builder
public class ExceptionDto {
    private final String message;
    public final HttpStatus statusCode;

    public static ResponseEntity<ExceptionDto> toResponseEntity(HttpStatus httpStatus, String message){

        return ResponseEntity
                .status(httpStatus)
                .body(ExceptionDto.builder()
                        .statusCode(httpStatus)
                        .message(message)
                        .build());
    }

}
