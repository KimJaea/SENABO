package com.senabo.exception.handler;

import com.senabo.exception.dto.ExceptionDto;
import com.senabo.exception.model.*;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

@Slf4j
@RestControllerAdvice(basePackages = "com.senabo")
public class GlobalExceptionHandler {

    //    @ExceptionHandler(value = Exception.class)
//    public ResponseEntity<ExceptionDto> exceptionHandler(Exception exception){
//        return ExceptionDto.
//    }
    @ExceptionHandler(DataException.class)
    protected ResponseEntity<?> handleDataException(DataException exception) {
        log.info("ExceptionHandler DataException 진입");
        exception.printStackTrace();
        return ExceptionDto.toResponseEntity(exception.getHttpStatus(), exception.getMessage());
    }

    @ExceptionHandler(TokenCheckFailException.class)
    protected ResponseEntity<?> handleTokenCheckFailException(TokenCheckFailException exception) {
        log.info("ExceptionHandler TokenCheckFailException 진입");
        exception.printStackTrace();
        return ExceptionDto.toResponseEntity(exception.getHttpStatus(), exception.getMessage());
    }

    @ExceptionHandler(TokenNotFoundException.class)
    protected ResponseEntity<?> handleTokenNotFoundException(TokenNotFoundException exception) {
        log.info("ExceptionHandler TokenNotFoundException 진입");
        exception.printStackTrace();
        return ExceptionDto.toResponseEntity(exception.getHttpStatus(), exception.getMessage());
    }

    @ExceptionHandler(UserAuthException.class)
    protected ResponseEntity<?> handleUserAuthException(UserAuthException exception) {
        log.info("ExceptionHandler UserAuthException 진입");
        exception.printStackTrace();
        return ExceptionDto.toResponseEntity(exception.getHttpStatus(), exception.getMessage());
    }

    @ExceptionHandler(UserException.class)
    protected ResponseEntity<?> handleUserException(UserException exception) {
        log.info("ExceptionHandler UserException 진입");
        exception.printStackTrace();
        return ExceptionDto.toResponseEntity(exception.getHttpStatus(), exception.getMessage());
    }


}
