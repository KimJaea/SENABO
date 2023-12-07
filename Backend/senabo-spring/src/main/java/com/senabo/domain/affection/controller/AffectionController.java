package com.senabo.domain.affection.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.affection.dto.request.AddAffectionRequest;
import com.senabo.domain.affection.entity.Affection;
import com.senabo.domain.affection.service.AffectionService;
import com.senabo.domain.affection.dto.response.AffectionResponse;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
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
import java.util.Optional;
import java.util.stream.Collectors;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/affection")
@Tag(name = "Affection", description = "Affection API Document")
public class AffectionController {
    private final AffectionService affectionService;
    private final ReportService reportService;
    private final MemberService memberService;

    @PostMapping("/save")
    @Operation(summary = "애정 지수 증가", description = "애정 지수가 입력한 수만큼 변화한다.")
    public ApiResponse<AffectionResponse> addAffection(@AuthenticationPrincipal UserDetails principal, @RequestBody AddAffectionRequest request) {
        AffectionResponse response = affectionService.createAffection(principal.getUsername(), request.type(), request.changeAmount());
        return ApiResponse.success("애정 지수 " + request.changeAmount() + " 변화 성공", response);
    }


    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = AffectionResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
        }
    )
    @Operation(summary = "애정 지수 전체 조회", description = "애정 지수 내역을 전체 조회한다.")
    public ApiResponse<List<AffectionResponse>> getAffection(@AuthenticationPrincipal UserDetails principal) {
        List<Affection> affection = affectionService.getAffection(principal.getUsername());
        if (affection.isEmpty()) return ApiResponse.fail("애정 지수 전체 조회 실패", null);
        List<AffectionResponse> reponse = affection.stream()
                .map(AffectionResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("애정 지수 전체 조회 성공", reponse);
    }


    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = AffectionResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "애정 지수 주간 조회", description = "애정 지수 내역을 주간 조회한다.")
    public ApiResponse<List<AffectionResponse>> getAffectionWeek(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional =  reportService.findReportWeek(member, week);
        if(reportOptional.isEmpty()) return ApiResponse.fail("애정 지수 " + week + "주차 조회 실패", null);
        List<Affection> affection = affectionService.getAffectionWeek(reportOptional.get(), member);
        List<AffectionResponse> response = affection.stream().map(AffectionResponse::from).collect(Collectors.toList());
        return ApiResponse.success("애정 지수 " + week + "주차 조회 성공", response);
    }

}
