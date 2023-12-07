package com.senabo.exception.model;


import com.senabo.exception.message.ExceptionMessage;
import lombok.Getter;
import org.springframework.http.HttpStatus;
@Getter
public class UserException extends RuntimeException {
    private HttpStatus httpStatus;
    private String message;
    public UserException(String error){super(error);}

    public UserException(ExceptionMessage exceptionMessage){
        this.httpStatus = exceptionMessage.getHttpStatus();
        this.message = exceptionMessage.getMessage();
    }
}
