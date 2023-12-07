package com.senabo.exception.model;


import com.senabo.exception.message.ExceptionMessage;
import lombok.Getter;
import org.springframework.http.HttpStatus;
@Getter
public class UserAuthException extends RuntimeException{
    private HttpStatus httpStatus;
    private String message;
    public UserAuthException(String error) {super(error);}

    public UserAuthException(ExceptionMessage exceptionMessage) {  this.httpStatus = exceptionMessage.getHttpStatus();
        this.message = exceptionMessage.getMessage();}
}
