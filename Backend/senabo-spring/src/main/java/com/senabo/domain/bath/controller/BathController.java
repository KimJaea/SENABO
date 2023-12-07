package com.senabo.domain.bath.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.affection.dto.response.AffectionResponse;
import com.senabo.domain.bath.dto.response.BathResponse;
import com.senabo.domain.bath.entity.Bath;
import com.senabo.domain.bath.service.BathService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/bath")
@Tag(name = "Bath", description = "Bath API Document")
public class BathController {
    
    private final BathService bathService;

    @PostMapping("/save")
    @Operation(summary = "목욕 생성", description = "목욕을 한 시각을 저장한다.")
    public ApiResponse<BathResponse> createBath(@AuthenticationPrincipal UserDetails principal){
        BathResponse response = bathService.createBath(principal.getUsername());
        return ApiResponse.success("목욕 저장 완료", response);
    }


    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = BathResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "목욕 조회", description = "목욕 내역을 전체 조회한다.")
    public ApiResponse<List<BathResponse>>  getBath(@AuthenticationPrincipal UserDetails principal){
        List<Bath> bath = bathService.getBath(principal.getUsername());
        if(bath.isEmpty()) return ApiResponse.fail("목욕 전체 조회 실패", null);
        List<BathResponse> response = bath.stream()
                .map(BathResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("목욕 전체 조회 성공", response);
    }


    @DeleteMapping("/remove")
    @Operation(summary = "목욕 내역 삭제", description = "목욕 내역을 삭제한다.")
    public ApiResponse<Object> removeBath(@AuthenticationPrincipal UserDetails principal) {
        bathService.removeBath(principal.getUsername());
        return ApiResponse.success("목욕 삭제 성공", null);
    }
}
