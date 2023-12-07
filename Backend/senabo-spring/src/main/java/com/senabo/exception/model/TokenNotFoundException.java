package com.senabo.exception.model;


import com.senabo.exception.message.ExceptionMessage;
import lombok.Getter;
import org.springframework.http.HttpStatus;
@Getter
public class TokenNotFoundException extends RuntimeException{
    private HttpStatus httpStatus;
    private String message;
    public TokenNotFoundException(ExceptionMessage exceptionMessage) {
        this.httpStatus = exceptionMessage.getHttpStatus();
        this.message = exceptionMessage.getMessage();
    }
}
